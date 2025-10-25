import { MessageHandler } from "discord-message-handler";
import { Infra } from "../models/infra.class";
import { Message } from "discord.js";
import { PokemonService } from "../services/pokemon.service";
import { dependencyInjectionContainer } from "../di-container";
import { MessageService } from "../services/message.service";
import { isNull, isNullOrUndefined } from "util";

export class CounterCommand {
    static setup(handler: MessageHandler) {
        handler.onCommand("!counters")
            .minArgs(1)
            // .allowedChannels(["670545170386780170"])
            // .whenInvalid({ replyToUser: true, minimumArgs: "Did you forget to type in the pokemon name?", allowedChannels: "Please use this command in the help channel."})
            .whenInvalid("ti ni just")
            .do(async (args: string[], rawArgs: string, message: Message) => {
                var pokemonService = dependencyInjectionContainer.get(PokemonService)
                let messageService: MessageService = dependencyInjectionContainer.get(MessageService)
                if (args.length == 1) {
                    if (args[0] == "list") {
                        var countersList = await pokemonService.getCountersList()
                        if (!isNullOrUndefined(countersList)) {
                            messageService.handlePokemonCounterListMessage(countersList)
                        } else {
                            message.delete()
                            message.channel.send("Couldnt recognize that command")
                        }
                    } else {
                        var searchResult: any = await pokemonService.searchPokemonCounter(args[0])
                        if (!isNull(searchResult) && searchResult.length > 0) {
                            messageService.handlePokemonCounterMessage(searchResult[0]) // take first
                        } else {
                            message.delete()
                            message.channel.send("Couldnt find that pokemon")
                        }
                    }
                } else if (args.length == 2) {
                    message.channel.send("Not implemented")
                } else {
                    message.channel.send("Not implemented")
                }
            })
    }
}