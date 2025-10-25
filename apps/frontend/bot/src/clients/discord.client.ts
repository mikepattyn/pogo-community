import { MessageHandler } from 'discord-message-handler';
import {
  Client,
  Message,
  MessageReaction,
  TextChannel,
  User,
} from 'discord.js';
import { injectable } from 'inversify';
import moment from 'moment';
import { isNullOrUndefined } from 'util';
import { CounterCommand } from '../commands/counter.command';
import { JoinCommand } from '../commands/join.command';
import { RaidCommand } from '../commands/raid.command';
import { RegisterRankCommand } from '../commands/register.command';
import { ScanRaidImageCommand } from '../commands/scanraidimage.command';
import { TestCommand } from '../commands/test.command';
import { IDataPlayer } from '../dbmodels/interfaces/IDataPlayer';
import { dependencyInjectionContainer } from '../di-container';
import { MessageReactionHandler } from '../message.reaction.handler';
import { ChannelIds } from '../models/channelIds.enum';
import { MessageService } from '../services/message.service';
import { ApiClient } from './apiClient';
const allowedEmojisRaid = ['', '', '', ''];
const allowedEmojisRaidExtra = [
  '1⃣',
  '2⃣',
  '3⃣',
  '4⃣',
  '5⃣',
  '6⃣',
  '7⃣',
  '8⃣',
  '9⃣',
];
const allowedEmojisRank = ['', '', '', ''];

@injectable()
export class DiscordClient {
  client: Client = new Client();
  handler: MessageHandler = new MessageHandler();
  messageReactionHandler: MessageReactionHandler = new MessageReactionHandler();

  channels: TextChannel[] = [];

  private apiClient: ApiClient =
    dependencyInjectionContainer.get<ApiClient>(ApiClient);
  private messageService: MessageService =
    dependencyInjectionContainer.get<MessageService>(MessageService);

  constructor() {
    // Setup commands
    this.setupCommands(this.handler);
  }

  login() {
    this.client.login(process.env.BOT_TOKEN);
  }
  async onReady() {
    this.client.on('ready', async () => {
      console.log(`Info: Logged in as ${this.client.user.tag}!`);
      const channel = this.getChannelById(ChannelIds.Welcome) as TextChannel;
      if (!isNullOrUndefined(channel)) {
        this.channels.push(channel);
      }
    });
  }

  onMessage() {
    this.client.on('message', async (message: Message) => {
      if (message.type === 'GUILD_MEMBER_JOIN') {
        const newPlayer: IDataPlayer = {
          Id: -1,
          DiscordId: message.author.id,
          DateJoined: moment(new Date()).format('YYYY/MM/DD HH:mm:ss'),
          FirstName: null,
          Nickname: null,
          Level: null,
          Team: null,
        };
        await this.apiClient.post('/players', newPlayer);

        console.log(`Member with id: ${newPlayer.DiscordId} joined discord.`);
      } else if (message.type === 'DEFAULT') {
        this.messageService.setMessage(message);
        this.handler.handleMessage(message);
      }
    });
  }
  onMessageReactionAdd() {
    this.client.on(
      'messageReactionAdd',
      async (reaction: MessageReaction, user: User) => {
        if (allowedEmojisRaid.some((x) => x === reaction.message.channel.id)) {
          this.messageReactionHandler.handleJoiningRaid(reaction, user);
        } else if (
          allowedEmojisRaidExtra.some((x) => x === reaction.message.channel.id)
        ) {
          this.messageReactionHandler.handleAddingExtra(reaction, user);
        } else if (
          allowedEmojisRank.some((x) => x === reaction.message.channel.id)
        ) {
          this.messageReactionHandler.handleJoiningRank(reaction, user);
        }
      }
    );
  }
  onMessageReactionRemove() {
    this.client.on(
      'messageReactionRemove',
      async (reaction: MessageReaction, user: User) => {
        if (allowedEmojisRaid.some((x) => x === reaction.message.channel.id)) {
          this.messageReactionHandler.handleLeavingRaid(reaction, user);
        } else if (
          allowedEmojisRaidExtra.some((x) => x === reaction.message.channel.id)
        ) {
          this.messageReactionHandler.handleRemovingExtra(reaction, user);
        } else if (
          allowedEmojisRank.some((x) => x === reaction.message.channel.id)
        ) {
          this.messageReactionHandler.handleLeavingRank(reaction, user);
        }
      }
    );
  }
  getChannelById(id: string) {
    return this.client.channels.get(id);
  }

  private setupCommands = (handler: MessageHandler) => {
    RaidCommand.setup(handler);
    RegisterRankCommand.setup(handler);
    CounterCommand.setup(handler);
    JoinCommand.setup(handler);
    ScanRaidImageCommand.setup(handler);
    TestCommand.setup(handler);
  };
}
