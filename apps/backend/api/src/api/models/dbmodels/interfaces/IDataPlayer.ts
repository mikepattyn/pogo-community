export interface IDataPlayer {
  Id: number;
  DiscordId: string;
  FirstName: string | null;
  Nickname: string | null;
  Level: number | null;
  Team: Teams | null;
  DateJoined: Date;
}

export enum Teams {
  Instinct,
  Mystic,
  Valor,
}
