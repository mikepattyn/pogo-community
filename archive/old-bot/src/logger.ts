import { injectable } from 'inversify';
import winston from 'winston';
import LokiTransport from 'winston-loki';

@injectable()
export class Logger {
  private logger: winston.Logger;

  constructor() {
    const transports: winston.transport[] = [
      new winston.transports.Console({
        format: winston.format.combine(
          winston.format.colorize(),
          winston.format.timestamp(),
          winston.format.printf(({ timestamp, level, message, ...meta }) => {
            return `${timestamp} [${level}]: ${message} ${Object.keys(meta).length ? JSON.stringify(meta) : ''}`;
          })
        )
      })
    ];

    // Add Loki transport if LOKI_URL is configured
    const lokiUrl = process.env.LOKI_URL;
    if (lokiUrl) {
      transports.push(
        new LokiTransport({
          host: lokiUrl,
          labels: {
            app: 'pogo-bot',
            environment: process.env.NODE_ENV || 'development'
          },
          json: true,
          format: winston.format.json()
        })
      );
    }

    this.logger = winston.createLogger({
      level: process.env.LOG_LEVEL || 'info',
      transports,
      defaultMeta: {
        service: 'pogo-bot'
      }
    });
  }

  async log(data: string) {
    this.logger.info(data);
    console.log('Logged: ', data);
  }

  debug(message: string, meta?: any) {
    this.logger.debug(message, meta);
  }

  info(message: string, meta?: any) {
    this.logger.info(message, meta);
  }

  warn(message: string, meta?: any) {
    this.logger.warn(message, meta);
  }

  error(message: string, error?: any) {
    this.logger.error(message, { error: error?.stack || error });
  }
}
