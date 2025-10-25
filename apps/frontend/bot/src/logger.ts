import { injectable } from 'inversify';
const { Logging } = require('@google-cloud/logging');

@injectable()
export class Logger {
  private projectId = process.env.CLOUD_SQL_CONNECTION_NAME;
  private logName = 'Pokebot.Bot.Debug';
  private logging = new Logging({ projectId: this.projectId });

  async log(data: string) {
    console.log(this.logging);
    const log = this.logging.log(this.logName);
    const metadata = {
      resource: { type: 'global' },
    };
    const entry = log.entry(metadata, { message: data });

    await log.write(entry);

    console.log('Logged: ', data);
  }

  debug(message: string) {
    console.log(`[DEBUG] ${message}`);
  }

  info(message: string) {
    console.log(`[INFO] ${message}`);
  }

  warn(message: string) {
    console.warn(`[WARN] ${message}`);
  }

  error(message: string) {
    console.error(`[ERROR] ${message}`);
  }
}
