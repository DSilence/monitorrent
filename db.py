from sqlalchemy import create_engine, event, Column, String, Integer, Table
import sqlalchemy.orm
from sqlalchemy.orm import sessionmaker, scoped_session
from sqlalchemy.ext.declarative import declarative_base
from alembic.migration import MigrationContext
from alembic.operations import Operations


class ContextSession(sqlalchemy.orm.Session):
    """:class:`sqlalchemy.orm.Session` which can be used as context manager"""
    @property
    def dialect(self):
        return self.bind.dialect

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        try:
            if exc_type is None:
                self.commit()
            else:
                self.rollback()
        finally:
            self.close()

Base = declarative_base()
session_factory = sessionmaker(class_=ContextSession)
DBSession = scoped_session(session_factory)
engine = None


def init_db_engine(connection_string, echo=False):
    global engine
    engine = create_engine(connection_string, echo=echo)

    # workaround for migrations on sqlite:
    # http://docs.sqlalchemy.org/en/latest/dialects/sqlite.html#pysqlite-serializable
    @event.listens_for(engine, 'connect')
    def do_connect(dbapi_connection, connection_record):
        # disable pysqlite's emitting of the BEGIN statement entirely.
        # also stops it from emitting COMMIT before any DDL.
        dbapi_connection.isolation_level = None

    @event.listens_for(engine, "begin")
    def do_begin(conn):
        # emit our own BEGIN
        conn.execute("BEGIN")

    session_factory.configure(bind=engine)

def create_db():
    Base.metadata.create_all(engine)

def row2dict(row, table=None):
    """
    Converts SQLAlchemy row object into dict

    :rtype : dict, tuple
    """
    if table is not None:
        keys = table.columns.keys()
        return {keys[i]: row[i] for i in range(0, len(row))}

    return {c.name: getattr(row, c.name) for c in row.__table__.columns}

CoreBase = declarative_base()


class Version(CoreBase):
    __tablename__ = 'plugin_versions'

    plugin = Column(String, nullable=False, primary_key=True)
    version = Column(Integer, nullable=False)


def get_version(name):
    with DBSession() as db:
        version = db.query(Version).filter(Version.plugin == name).first()
        if not version:
            return -1
        return version.version

def set_version(name, version):
    with DBSession() as db:
        db_version = db.query(Version).filter(Version.plugin == name).first()
        if not db_version:
            db_version = Version(plugin=name)
            db.add(db_version)
        db_version.version = version
        db.commit()

def upgrade(plugins, upgrades):
    CoreBase.metadata.create_all(engine)

    def operation_factory(session=None):
        if session is None:
            session = DBSession()
        migration_context = MigrationContext.configure(session)
        return MonitorrentOperations(session, migration_context)

    for name, plugins in plugins.items():
        upgrade_func = upgrades.get(name, None)
        if not upgrade_func:
            continue
        version = get_version(name)
        try:
            version = upgrade_func(engine, operation_factory, version)
            set_version(name, version)
        except Exception as e:
            print e


class MonitorrentOperations(Operations):
    def __init__(self, db, migration_context, impl=None):
        self.db = db
        super(MonitorrentOperations, self).__init__(migration_context, impl)

    def create_table(self, *args, **kw):
        if len(args) > 0 and type(args[0]) is Table:
            table = args[0]
            columns = [c.copy() for c in table.columns]
            if len(args) > 1:
                columns = columns + list(args[1:])
            return super(MonitorrentOperations, self).create_table(table.name, *columns, **kw)
        return super(MonitorrentOperations, self).create_table(*args, **kw)

    def upgrade_to_base_topic(self, v0, v1, polymorphic_identity, topic_mapping=None, column_renames=None):
        from plugins import Topic

        self.create_table(v1)
        topics = self.db.query(v0)
        for topic in topics:
            raw_topic = row2dict(topic, v0)
            # insert into topics
            topic_values = {c: v for c, v in raw_topic.items() if c in Topic.__table__.c and c != 'id'}
            topic_values['type'] = polymorphic_identity
            if topic_mapping:
                topic_mapping(topic_values, raw_topic)
            result = self.db.execute(Topic.__table__.insert(), topic_values)

            # get topic.id
            inserted_id = result.inserted_primary_key[0]

            # insert into v1 table
            concrete_topic = {c: v for c, v in raw_topic.items() if c in v1.c}
            concrete_topic['id'] = inserted_id
            if column_renames:
                column_renames(concrete_topic, raw_topic)
            self.db.execute(v1.insert(), concrete_topic)
        # drop original table
        self.drop_table(v0.name)
        # rename new created table to old one
        self.rename_table(v1.name, v0.name)

    def __enter__(self):
        self.db.__enter__()
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        self.db.__exit__(exc_type, exc_val, exc_tb)
