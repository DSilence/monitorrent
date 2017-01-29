import bunyan from 'bunyan'
import App from './App'

var log = bunyan.createLogger({
  name: 'MonitorrentLogger',
  streams: [
    {
      type: 'rotating-file',
      path: App.getPath('userData') + '/log.txt',
      period: '1d',
      count: 10
    }
  ]
})

export default log
