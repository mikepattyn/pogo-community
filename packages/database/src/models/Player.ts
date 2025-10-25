export enum Teams {
  Instinct,
  Mystic,
  Valor,
}

export interface IPlayer {
  Id: number;
  DiscordId: string;
  FirstName: string | null;
  Nickname: string | null;
  Level: number | null;
  Team: Teams | null;
  DateJoined: Date;
}

export class Player implements IPlayer {
  Id: number;
  DiscordId: string;
  FirstName: string | null;
  Nickname: string | null;
  Level: number | null;
  Team: Teams | null;
  DateJoined: Date;

  constructor(data: IPlayer) {
    this.Id = data.Id;
    this.DiscordId = data.DiscordId;
    this.FirstName = data.FirstName;
    this.Nickname = data.Nickname;
    this.Level = data.Level;
    this.Team = data.Team;
    this.DateJoined = data.DateJoined;
  }
}

