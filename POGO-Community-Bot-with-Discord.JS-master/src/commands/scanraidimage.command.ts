import { MessageHandler } from "discord-message-handler";
import { Message, TextChannel, RichEmbed } from "discord.js";
import { isNullOrUndefined, isNull } from "util";
import { dependencyInjectionContainer } from "../di-container";
import { ChannelIds } from "../models/channelIds.enum";
import { RaidStore } from "../stores/raid.store";
import { PokemonStore } from "../stores/pokemon.store";
import { pokemon } from "./../resources/statics/pokemon"
import { GymInfo } from "../models/GymInfo";
import { DiscordHelper } from "../helpers/discord.helper";
import { MessageService } from "../services/message.service";
import { ApiClient } from "../clients/apiClient";
import { AxiosResponse } from "axios";
import { Logger } from "../logger";

const arrayWithGenerations: any[] = [pokemon.gen1, pokemon.gen2, pokemon.gen3, pokemon.gen4, pokemon.gen5];
const allowedChannels: string[] = ["668134717614456895"]

var uuidv4 = require('uuid/v4')
export class ScanRaidImageCommand {
    private static messageService: MessageService = dependencyInjectionContainer.get<MessageService>(MessageService);
    private static pokemonStore: PokemonStore = dependencyInjectionContainer.get<PokemonStore>(PokemonStore);
    private static client: ApiClient = dependencyInjectionContainer.get<ApiClient>(ApiClient);
    private static logger: Logger = dependencyInjectionContainer.get<Logger>(Logger)
    static setup(handler: MessageHandler) {
        handler.onCommand("!scan")
            .minArgs(1)
            .matches('([a-z]|[A-Z])([1-5]{1})')
            // .allowedChannels(allowedChannels)
            // .whenInvalid({
            //     replyToUser: true,
            //     minimumArgs: "Heb je vergeten de T1-5 toe te voegen?",
            //     regexPattern: "Heb je vergeten de T1-5 toe te voegen?",
            //     allowedChannels: `Het commando !scan T1-5 enkel toegestaan in ${this.messageService.message?.guild.channels.filter(x => ["668134717614456895"].indexOf(x.id) > -1).map(channel => channel.name)}`
            // })
            .whenInvalid("ti ni goe")
            .deleteInvocation()
            .do(async (args: string[], rawArgs: string, message: Message) => {
                var returnMessage: RichEmbed | string = "Ti etwa hjil skjif gegoan"

                var tiers = 0
                if (!isNullOrUndefined(args) && !isNullOrUndefined(args[0]) && args[0].length === 2) {
                    tiers = Number([args[0][1]])
                }
                if (isNullOrUndefined(this.client) || isNullOrUndefined(this.pokemonStore)) {
                    return this.handleError(message, "Something went wrong. Please try again. If this problem persists, please contact support.")
                }
                if (message.channel.id != ChannelIds.RaidScanChannel.toString()) {
                    return this.handleError(message, "You are not allowed to do this here!")
                }
                var attachment = message.attachments.first();
                if (isNullOrUndefined(attachment.url) && attachment.url != "") {
                    return this.handleError(message, "Something went wrong fetching attachement url. Please try again. If this problem persists, please contact support.")
                }
                var response: AxiosResponse = await this.client.post("/scans", { url: attachment.url })
                var textResults: string[] | null = null

                if (response.status >= 200 && response.status < 300) {
                    textResults = response.data.textResults
                } else {
                    this.logger.log(CustomErrors[response.data.error])
                }

                if (isNullOrUndefined(textResults)) {
                    return this.handleError(message, "Something went wrong getting text result from your image. Please try again. If this problem persists, please contact support.")
                }

                var resultWithNumbers: any[] = []
                var resultWithoutNumbers: any[] = []

                // Split arrays into string with and without numbers
                textResults!.forEach((result: string) => {
                    if (new RegExp("[0-9]").test(result))
                        resultWithNumbers.push(result)
                    else
                        resultWithoutNumbers.push(result);
                })

                // Check if any contains EX RAID GYM
                var exRaidGym: boolean = resultWithoutNumbers.filter(x => x == "EX RAID GYM").length == 1
                if (exRaidGym) {
                    resultWithoutNumbers = resultWithoutNumbers.filter(result => result != "EX RAID GYM")
                }
                // Check if any is a pokemon name <- means if we find a match the egg is already hatched
                var pokemonMatch: any = null;

                resultWithoutNumbers.forEach((textResult: string) => {
                    if (pokemonMatch == null) {
                        var resultLowerCased: string = textResult.toLowerCase();
                        // checking each generation their pokemon_species
                        if (pokemonMatch == null) {
                            arrayWithGenerations.forEach((generation: any) => {
                                if (pokemonMatch == null) {
                                    // checking every pokemon in that generation
                                    generation.pokemon_species.forEach((mon: any) => {
                                        if (mon.name === resultLowerCased) {
                                            pokemonMatch = textResult
                                        }
                                    })
                                }
                            })
                        }
                    }
                })

                // Get the time left until hatch or disapear
                var timeLeft = resultWithNumbers.filter(x => ScanRaidImageCommand.getNthOccurencesOf(x, ":") == 2)[0].substring(0, 8)

                // Determine isHatched based on found a pokemon name
                var isHatched = !isNull(pokemonMatch)

                var gymName = ""

                if (isHatched) {
                    // asume the gym name is above the pokemon name
                    var findRes = resultWithoutNumbers.filter(x => x.indexOf(pokemonMatch.substring(2)) > -1)[0]
                    var index = resultWithoutNumbers.indexOf(findRes) - 1
                    gymName = index > 0 ? `${resultWithoutNumbers[index - 1]} ${resultWithoutNumbers[index]}` : resultWithoutNumbers[index]

                } else {
                    // in 4 out 5 times it was this first element so taking first
                    var searchResult: string = textResults.filter(x => textResults!.indexOf(timeLeft) > -1)[0]
                    var index = resultWithoutNumbers.indexOf(searchResult)
                    gymName = index > 0 ? `${resultWithoutNumbers[index - 1]} ${resultWithoutNumbers[index]}` : resultWithoutNumbers[index]
                }

                var info = new GymInfo([gymName, pokemonMatch, timeLeft])
                returnMessage = new RichEmbed()

                if (isHatched) {
                    returnMessage.setTitle(`🗡️ T${tiers} ${info.pokemon} ${info.titel}`)
                    returnMessage.setDescription(`Gym: ${info.titel!}.\nIt disapears at ${info.dtEnd}`);
                }
                else {
                    returnMessage.setTitle(`🗡️ T${tiers} ${info.titel}`)
                    returnMessage.setDescription(`Gym: ${info.titel!}.\nIt hatches at ${info.dtEnd}`);
                }

                returnMessage.setTimestamp()
                    .setFooter(`${DiscordHelper.findDisplayName(message)}`, `${DiscordHelper.findDisplayAvatar(message)}`)
                    .setDescription(`Reageer met 👍 om te joinen\nReageer met:\n${['1⃣', '2⃣', '3⃣', '4⃣', '5⃣', '6⃣', '7⃣', '8⃣', '9⃣'].join(' ')}\nom aan te geven dat je extra accounts of spelers mee hebt.\nLocatie: https://goo.gl/maps/Ty9VrxWW9uSYGhgy6`)
                    .setThumbnail("https://pokemongohub.net/wp-content/uploads/2019/10/darkrai-halloween.jpg")
                    .setColor(isHatched ? "00FF00" : "FFA500")

                this.handleSuccess(message, returnMessage, uuidv4(), new Date(), info.titel!, info.pokemon!, isHatched, tiers);
            })
    }

    private static async handleError(message: Message, error: string): Promise<void> {
        this.logger.log(`ErrorLocation: ${ScanRaidImageCommand.name}\nError: ${error}\nMessage: ${message}`)
        message.author.send(error);
    }
    private static async handleSuccess(message: Message, returnMessage: RichEmbed, guid: string, dateEnd: Date, gymName: string, pokemonName: string, isHatched: boolean, tiers: number) {
        var store: RaidStore = new RaidStore();
        await store.insert({
            Guid: guid,
            DateEnd: dateEnd,
            GymName: gymName,
            PokemonName: pokemonName,
            IsHatched: isHatched,
            Tiers: tiers
        });
        (message.guild.channels.get('655418834358108220') as TextChannel).sendEmbed(returnMessage)
    }
    private static getNthOccurencesOf(input: string, match: string) {
        var count = 0;
        for (var i = 0; i < input.length; i++) {
            if (input.charAt(i) === match) {
                count++;
            }
        }
        return count;
    }
}

export enum CustomErrors {
    UNDEFINED_OR_EMPTY_URL,
    UNDEFINED_OR_EMPTY_RESULTS
}