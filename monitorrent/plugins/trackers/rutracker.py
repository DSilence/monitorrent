#!/usr/bin/env python
# -*- coding: utf-8 -*-
import re
from requests import Session
import requests
from sqlalchemy import Column, Integer, String, ForeignKey
from monitorrent.db import Base, DBSession
from monitorrent.plugins import Topic
from monitorrent.plugin_managers import register_plugin
from monitorrent.utils.soup import get_soup
from monitorrent.utils.bittorrent import Torrent
from monitorrent.plugins.trackers import TrackerPluginBase, WithCredentialsMixin, ExecuteWithHashChangeMixin, LoginResult

PLUGIN_NAME = 'rutracker.org'


class RutrackerCredentials(Base):
    __tablename__ = "rutracker_credentials"

    username = Column(String, primary_key=True)
    password = Column(String, primary_key=True)
    uid = Column(String, nullable=True)
    bb_data = Column(String, nullable=True)


class RutrackerTopic(Topic):
    __tablename__ = "rutracker_topics"

    id = Column(Integer, ForeignKey('topics.id'), primary_key=True)
    hash = Column(String, nullable=True)

    __mapper_args__ = {
        'polymorphic_identity': PLUGIN_NAME
    }


class RutrackerLoginFailedException(Exception):
    def __init__(self, code, message):
        self.code = code
        self.message = message


class RutrackerTracker(object):
    login_url = "http://login.rutracker.org/forum/login.php"
    profile_page = "http://rutracker.org/forum/profile.php?mode=viewprofile&u={}"
    _regex = re.compile(ur'^http://w*\.*rutracker.org/forum/viewtopic.php\?t=(\d+)(/.*)?$')
    uid_regex = re.compile(ur'\d*-(\d*)-.*')
    title_header = u':: rutracker.org'

    def __init__(self, uid=None, bb_data=None):
        self.uid = uid
        self.bb_data = bb_data

    def setup(self, uid, bb_data):
        self.uid = uid
        self.bb_data = bb_data

    def can_parse_url(self, url):
        return self._regex.match(url) is not None

    def parse_url(self, url):
        match = self._regex.match(url)
        if match is None:
            return None

        # without slash response gets fucked up
        if not url.endswith("/"):
            url += "/"
        r = requests.get(url, allow_redirects=False)
        if r.status_code != 200:
            return None

        soup = get_soup(r.text)
        title = soup.h1.text.strip()
        if title.lower().endswith(self.title_header):
            title = title[:-len(self.title_header)].strip()

        return {'original_name': title}

    def login(self, username, password):
        s = Session()
        data = {"login_username": username, "login_password": password, 'login': u'Âõîä'.encode("cp1252")}
        login_result = s.post(self.login_url, data)
        if login_result.url.startswith(self.login_url):
            # TODO get error info (although it shouldn't contain anything useful
            raise RutrackerLoginFailedException(1, "Invalid login or password")
        else:
            bb_data = s.cookies.get('bb_data')
            if not bb_data:
                raise RutrackerLoginFailedException(2, "Failed to retrieve cookie")

            self.bb_data = bb_data
            self.uid = self.uid_regex.match(bb_data).group(1)

    def verify(self):
        if not self.uid:
            return False
        cookies = self.get_cookies()
        if not cookies:
            return False
        profile_page_url = self.profile_page.format(self.uid)
        profile_page_result = requests.get(profile_page_url, cookies=cookies)
        return profile_page_result.url == profile_page_url

    def get_cookies(self):
        if not self.bb_data:
            return False
        return {'bb_data': self.bb_data}

    def get_hash(self, url):
        download_url = self.get_download_url(url)
        if not download_url:
            return None
        cookies = self.get_cookies()
        if not cookies:
            return None
        r = requests.post(download_url, cookies=cookies)
        t = Torrent(r.content)
        return t.info_hash

    def get_id(self, url):
        match = self._regex.match(url)
        if match is None:
            return None

        return match.group(1)

    # noinspection PyShadowingBuiltins
    def get_download_url(self, url):
        id = self.get_id(url)
        if id is None:
            return None
        return "http://dl.rutracker.org/forum/dl.php?t=" + id


class RutrackerPlugin(WithCredentialsMixin, ExecuteWithHashChangeMixin, TrackerPluginBase):
    tracker = RutrackerTracker()
    topic_class = RutrackerTopic
    credentials_class = RutrackerCredentials
    topic_form = [{
        'type': 'row',
        'content': [{
            'type': 'text',
            'model': 'display_name',
            'label': 'Name',
            'flex': 100
        }]
    }]

    def login(self):
        with DBSession() as db:
            cred = db.query(self.credentials_class).first()
            if not cred:
                return LoginResult.CredentialsNotSpecified
            username = cred.username
            password = cred.password
            if not username or not password:
                return LoginResult.CredentialsNotSpecified
        try:
            self.tracker.login(username, password)
            with DBSession() as db:
                cred = db.query(self.credentials_class).first()
                if not cred:
                    cred = self.credentials_class()
                    db.add(cred)
                cred.uid = self.tracker.uid
                cred.bb_data = self.tracker.bb_data
            return LoginResult.Ok
        except RutrackerLoginFailedException as e:
            if e.code == 1:
                return LoginResult.IncorrentLoginPassword
            return LoginResult.Unknown
        except Exception as e:
            # TODO: Log unexpected excepton
            return LoginResult.Unknown

    def verify(self):
        with DBSession() as db:
            cred = db.query(self.credentials_class).first()
            if not cred:
                return False
            username = cred.username
            password = cred.password
            if not username or not password or not cred.uid or not cred.bb_data:
                return False
            self.tracker.setup(cred.uid, cred.bb_data)
        return self.tracker.verify()

    def can_parse_url(self, url):
        return self.tracker.can_parse_url(url)

    def parse_url(self, url):
        return self.tracker.parse_url(url)

    def _prepare_request(self, topic):
        headers = {'referer': topic.url, 'host': "dl.rutracker.org"}
        cookies = self.tracker.get_cookies()
        request = requests.Request('POST', self.tracker.get_download_url(topic.url), headers=headers, cookies=cookies)
        return request.prepare()


register_plugin('tracker', PLUGIN_NAME, RutrackerPlugin())
