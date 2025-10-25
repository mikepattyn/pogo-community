import { injectable } from "inversify";
const { Logging } = require('@google-cloud/logging');


@injectable()
export class Logger {
    private projectId = process.env.CLOUD_SQL_CONNECTION_NAME
    private logName = 'Pokebot.Api.Debug'
    private logging = new Logging({ projectId: this.projectId })

    async log(data: string) {
        var log = this.logging.log(this.logName)
        const metadata = {
            resource: { type: 'global' }
        }
        const entry = log.entry(metadata, { message: data })
        await log.write(entry)
    }
}