'use strict'

import { app, BrowserWindow, Menu, Tray } from 'electron'

let mainWindow
let tray
const winURL = process.env.NODE_ENV === 'development'
  ? `http://localhost:${require('../../../config').port}`
  : `file://${__dirname}/index.html`

function createWindow () {
  /**
   * Initial window options
   */
  mainWindow = new BrowserWindow({
    height: 600,
    width: 800
  })

  mainWindow.loadURL(winURL)

  mainWindow.on('closed', () => {
    mainWindow = null
  })

  // eslint-disable-next-line no-console
  console.log('mainWindow opened')
}

function createTray () {
  tray = new Tray('app/icons/Monitorrent.png')
  const contextMenu = Menu.buildFromTemplate([
    { label: 'Start', type: 'normal' },
    { label: 'Stop', type: 'normal' },
    { label: 'Restart', type: 'normal' }
  ])
  tray.setToolTip('Monitorrent.')
  tray.setContextMenu(contextMenu)
}

app.on('ready', createWindow)

app.on('ready', createTray)

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow()
  }
})
