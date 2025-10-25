export interface IRaidParticipant {
  Id: number;
  RaidId: number;
  PlayerId: number;
  JoinedAt: Date;
}

export class RaidParticipant implements IRaidParticipant {
  Id: number;
  RaidId: number;
  PlayerId: number;
  JoinedAt: Date;

  constructor(data: IRaidParticipant) {
    this.Id = data.Id;
    this.RaidId = data.RaidId;
    this.PlayerId = data.PlayerId;
    this.JoinedAt = data.JoinedAt;
  }
}
