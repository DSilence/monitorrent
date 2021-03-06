from gevent import monkey
monkey.patch_all()

import flask
from flask import Flask, redirect
from flask_restful import Resource, Api, abort, reqparse, request
from engine import Logger, EngineRunner
from db import init_db_engine, create_db, upgrade
from plugin_managers import load_plugins, get_all_plugins, upgrades, TrackersManager, ClientsManager
from flask_socketio import SocketIO, emit

init_db_engine("sqlite:///monitorrent.db", True)
load_plugins()
upgrade(get_all_plugins(), upgrades)
create_db()

tracker_manager = TrackersManager()
clients_manager = ClientsManager()

static_folder = "webapp"
app = Flask(__name__, static_folder=static_folder, static_url_path='')
app.config['SECRET_KEY'] = 'secret!'
socketio = SocketIO(app)

class EngineWebSocketLogger(Logger):
    def started(self):
        socketio.emit('started', namespace='/execute')

    def finished(self, finish_time, exception):
        args = {
            'finish_time': finish_time.isoformat(),
            'exception': exception.message if exception else None
        }
        socketio.emit('finished', args, namespace='/execute')

    def info(self, message):
        self.emit('info', message)

    def failed(self, message):
        self.emit('failed', message)

    def downloaded(self, message, torrent):
        self.emit('downloaded', message, size=len(torrent))

    def emit(self, level, message, **kwargs):
        data = {'level': level, 'message': message}
        data.update(kwargs)
        socketio.emit('log', data, namespace='/execute')


engine_runner = EngineRunner(EngineWebSocketLogger(), tracker_manager, clients_manager)

class Torrents(Resource):
    url_parser = reqparse.RequestParser()

    def __init__(self):
        super(Torrents, self).__init__()
        self.url_parser.add_argument('url', required=True)

    def get(self):
        return tracker_manager.get_watching_torrents()

    def delete(self):
        args = self.url_parser.parse_args()
        deleted = tracker_manager.remove_watch(args.url)
        if not deleted:
            abort(404, message='Torrent \'{}\' doesn\'t exist'.format(args.url))
        return None, 204

    def post(self):
        args = self.url_parser.parse_args()
        added = tracker_manager.add_watch(args.url)
        if not added:
            abort(400, message='Can\'t add torrent: \'{}\''.format(args.url))
        return None, 201


class Clients(Resource):
    def get(self, client):
        result = clients_manager.get_settings(client)
        if not result:
            abort(404, message='Client \'{}\' doesn\'t exist'.format(client))
        return result

    def put(self, client):
        settings = request.get_json()
        clients_manager.set_settings(client, settings)
        return None, 204


class ClientList(Resource):
    def get(self):
        return [{'name': c.name} for c in clients_manager.clients]


class Trackers(Resource):
    def get(self, tracker):
        result = tracker_manager.get_settings(tracker)
        if not result:
            abort(404, message='Client \'{}\' doesn\'t exist'.format(tracker))
        return result

    def put(self, tracker):
        settings = request.get_json()
        tracker_manager.set_settings(tracker, settings)
        return None, 204


class TrackerList(Resource):
    def get(self):
        return [{'name': t.name} for t in tracker_manager.trackers
                if hasattr(t, 'get_settings') and hasattr(t, 'set_settings')]


class Execute(Resource):
    def get(self):
        return {
            "interval": engine_runner.interval,
            "last_execute": engine_runner.last_execute.isoformat() if engine_runner.last_execute else None
        }

@socketio.on('execute', namespace='/execute')
def execute():
    engine_runner.execute()

@app.route('/')
def index():
    return app.send_static_file('index.html')

@app.route('/api/parse')
def parse_url():
    url = request.args['url']
    title = tracker_manager.parse_url(url)
    if title:
        return flask.jsonify(**title)
    abort(400, message='Can\' parse url: \'{}\''.format(url))

@app.route('/api/check_client')
def check_client():
    client = request.args['client']
    return '', 204 if clients_manager.check_connection(client) else 500

@app.route('/api/check_tracker')
def check_tracker():
    client = request.args['tracker']
    return '', 204 if tracker_manager.check_connection(client) else 500

@socketio.on_error_default
def default_error_handler(e):
    print e

api = Api(app)
api.add_resource(Torrents, '/api/torrents')
api.add_resource(ClientList, '/api/clients')
api.add_resource(Clients, '/api/clients/<string:client>')
api.add_resource(TrackerList, '/api/trackers')
api.add_resource(Trackers, '/api/trackers/<string:tracker>')
api.add_resource(Execute, '/api/execute')

if __name__ == '__main__':
    #app.run(debug=True)
    socketio.run(app)
