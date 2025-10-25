export interface IRaid {
  Id: number;
  GymId: number;
  Pokemon: string;
  Level: number;
  StartTime: Date;
  EndTime: Date;
  CreatedBy: number | null;
  CreatedAt: Date;
  UpdatedAt: Date;
}

export class Raid implements IRaid {
  Id: number;
  GymId: number;
  Pokemon: string;
  Level: number;
  StartTime: Date;
  EndTime: Date;
  CreatedBy: number | null;
  CreatedAt: Date;
  UpdatedAt: Date;

  constructor(data: IRaid) {
    this.Id = data.Id;
    this.GymId = data.GymId;
    this.Pokemon = data.Pokemon;
    this.Level = data.Level;
    this.StartTime = data.StartTime;
    this.EndTime = data.EndTime;
    this.CreatedBy = data.CreatedBy;
    this.CreatedAt = data.CreatedAt;
    this.UpdatedAt = data.UpdatedAt;
  }
}

