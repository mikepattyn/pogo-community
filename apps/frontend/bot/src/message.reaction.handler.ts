import { MessageReaction, User } from 'discord.js';

export class MessageReactionHandler {
  constructor() {}
  handleJoiningRaid(_reaction: MessageReaction, _user: User) {}
  handleLeavingRaid(_reaction: MessageReaction, _user: User) {}
  handleAddingExtra(_reaction: MessageReaction, _user: User) {}
  handleRemovingExtra(_reaction: MessageReaction, _user: User) {}
  handleJoiningRank(_reaction: MessageReaction, _user: User) {}
  handleLeavingRank(_reaction: MessageReaction, _user: User) {}
}
