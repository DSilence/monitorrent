import ChildProcess from 'child_process'
import log from '../logger'

export const stopped = 'STOPPED'
export const started = 'STARTED'
/* @flow */
export class MonitorrentProcessor {
  pythonFileLocation: string;
  scriptLocation: string;
  process: mixed;
  status: string;

  constructor (pythonFileLocation: string, scriptLocation: string) {
    this.pythonFileLocation = pythonFileLocation
    this.scriptLocation = scriptLocation
  }

  startProcess (): void {
    log.info({pythonFileLocation: this.pythonFileLocation, scriptLocation: this.scriptLocation}, 'Starting Monitorrent python process')
    this.process = ChildProcess.spawn(this.pythonFileLocation, this.scriptLocation)

    this.process.on('close', (code, signal) => {
      log.info({exitCode: this.exitCode, signal: this.signal}, 'Python process has exited')
      this.status = stopped
    })

    this.process.on('error', (error) => {
      log.error({error: error}, 'The process has encountered an error')
    })

    this.process.stdout.on('data', (data) => {
      log.debug(data)
    })
    this.process = started
  }

  stopProcess (): void {
    log.info('Stopping monitorrent process')
    this.process.kill()
  }

  restart (): void {
    this.stopProcess()
    this.startProcess()
  }

  getStatus (): string {
    return this.status
  }
}
