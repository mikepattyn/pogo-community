import { MessageHandler } from 'discord-message-handler'
import { Message } from 'discord.js';
import { dependencyInjectionContainer } from '../di-container';
import { ApiClient } from '../clients/apiClient';
import { AxiosResponse } from 'axios';

export class TestCommand {
    private static client: ApiClient = dependencyInjectionContainer.get<ApiClient>(ApiClient);

    static setup(handler: MessageHandler) {
        handler.onCommand("!debug")
            .minArgs(0)
            .do(async (args: string[], rawArgs: string, message: Message) => {
                var result: AxiosResponse = await this.client.post("/scans", { url: 'https://i.ebayimg.com/images/g/1oYAAOSwPg9cONV-/s-l1600.jpg' })
                if (result.status == 200) {
                    await message.author.send(result.data.textResults.join(" "))
                } else {
                    await message.author.send("Couldnt do that: ")
                }
            })
    }
}
