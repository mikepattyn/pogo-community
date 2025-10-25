export interface IGym {
  Id: number;
  Name: string;
  LocationId: number;
  ExGym: boolean;
  CreatedAt: Date;
  UpdatedAt: Date;
}

export class Gym implements IGym {
  Id: number;
  Name: string;
  LocationId: number;
  ExGym: boolean;
  CreatedAt: Date;
  UpdatedAt: Date;

  constructor(data: IGym) {
    this.Id = data.Id;
    this.Name = data.Name;
    this.LocationId = data.LocationId;
    this.ExGym = data.ExGym;
    this.CreatedAt = data.CreatedAt;
    this.UpdatedAt = data.UpdatedAt;
  }
}

