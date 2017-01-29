import * as types from './mutation-types'
import request from 'request'
import fs from 'fs'
import ChildProcess from 'child_process'

export const downloadAndInstall = async ({ commit }) => {
  await downloadInstaller({ commit })
  await doInstallWindows({ commit })
}

export const downloadInstaller = ({ commit }) => {
  let receivedBytes = 0
  let totalBytes = 0

  return new Promise((resolve, reject) => {
    let req = request({
      method: 'GET',
      uri: 'https://github.com/werwolfby/monitorrent/releases/download/1.1.1/MonitorrentInstaller-1.1.1.msi'
    })

    let out = fs.createWriteStream('Monitorrent.msi')
    req.on('response', function (data) {
      totalBytes = parseInt(data.headers['content-length'])
    })

    req.on('data', function (chunk) {
      // Update the received bytes
      receivedBytes += chunk.length
      commit(types.PROGRESS_CHANGED, { progress: receivedBytes, total: totalBytes })
    })

    req.on('error', (error) => {
      reject(error)
    })

    req.on('end', function () {
      commit(types.DOWNLOAD_FINISHED)
      resolve()
    })
    req.pipe(out)
  })
}

export const doInstallWindows = ({ commit }) => {
  return new Promise((resolve, reject) => {
    fs.rename('Monitorrent.msi', 'install_scripts\\Monitorrent.msi', (error) => {
      if (error !== null) {
        reject(error)
      }
      ChildProcess.exec('powershell .\\install.ps1', {cwd: 'C:\\Personal\\monitorrent\\desktop\\install_scripts'}, (error, stdout, stderr) => {
        console.log('stdout: ' + stdout)
        console.log('stderr: ' + stderr)
        if (error !== null) {
          reject(error)
        }
        // fs.unlink('install_scripts\\Monitorrent.msi')
        commit(types.INSTALL_FINISHED)
        resolve()
      })
      commit(types.INSTALL_STARTED)
    })
  })
}
