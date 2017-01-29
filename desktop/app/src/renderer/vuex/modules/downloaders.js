import * as types from '../mutation-types'

const DOWNLOAD_STATUS_DOWNLOADING = 'Downloading'
const DOWNLOAD_FINISHED = 'Download finished'
export const INSTALL_STARTED = 'Installing'
export const INSTALL_FINISHED = 'Install finished'

export class DownloaderStateType {
  progress: number;
  downloadStatus: string;
  constructor () {
    this.downloadStatus = ''
    this.progress = 0
  }
}

const getters = {
  progress: state => state.progress,
  downloadStatus: state => state.downloadStatus,
  isInstalling: state => state.downloadStatus === INSTALL_STARTED
}

const mutations = {
  [types.PROGRESS_CHANGED] (state: DownloaderStateType, payload) {
    state.progress = payload.progress / payload.total * 100.0
    state.downloadStatus = DOWNLOAD_STATUS_DOWNLOADING
  },
  [types.DOWNLOAD_FINISHED] (state: DownloaderStateType) {
    state.progress = 0
    state.downloadStatus = DOWNLOAD_FINISHED
  },
  [types.INSTALL_STARTED] (state: DownloaderStateType) {
    state.downloadStatus = INSTALL_STARTED
  },
  [types.INSTALL_FINISHED] (state: DownloaderStateType) {
    state.downloadStatus = INSTALL_FINISHED
  }
}

const state: DownloaderStateType = new DownloaderStateType()

export default {
  state,
  getters,
  mutations
}
