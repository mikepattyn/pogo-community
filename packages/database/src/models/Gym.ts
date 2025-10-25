export interface IGym {
  Id: number;
  Name: string;
  LocationId: number;
}

export class Gym implements IGym {
  Id: number;
  Name: string;
  LocationId: number;

  constructor(data: IGym) {
    this.Id = data.Id;
    this.Name = data.Name;
    this.LocationId = data.LocationId;
  }
}

