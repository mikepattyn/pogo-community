export enum Teams {
  Instinct,
  Mystic,
  Valor,
}

export interface IPlayer {
  Id: number;
  Name: string;
  Team: string | null;
  Level: number;
  TrainerCode: string | null;
  DiscordId?: string; // Optional for backward compatibility
  CreatedAt: Date;
  UpdatedAt: Date;
}

export class Player implements IPlayer {
  Id: number;
  Name: string;
  Team: string | null;
  Level: number;
  TrainerCode: string | null;
  DiscordId?: string;
  CreatedAt: Date;
  UpdatedAt: Date;

  constructor(data: IPlayer) {
    this.Id = data.Id;
    this.Name = data.Name;
    this.Team = data.Team;
    this.Level = data.Level;
    this.TrainerCode = data.TrainerCode;
    this.DiscordId = data.DiscordId;
    this.CreatedAt = data.CreatedAt;
    this.UpdatedAt = data.UpdatedAt;
  }
}
