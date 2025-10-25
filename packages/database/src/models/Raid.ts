export interface IRaid {
  Id: number;
  MessageId: string;
  GymId: number;
  CreatedAt: Date;
  Tiers: number;
  TimeRemaining: number;
}

export class Raid implements IRaid {
  Id: number;
  MessageId: string;
  GymId: number;
  CreatedAt: Date;
  Tiers: number;
  TimeRemaining: number;

  constructor(data: IRaid) {
    this.Id = data.Id;
    this.MessageId = data.MessageId;
    this.GymId = data.GymId;
    this.CreatedAt = data.CreatedAt;
    this.Tiers = data.Tiers;
    this.TimeRemaining = data.TimeRemaining;
  }
}

