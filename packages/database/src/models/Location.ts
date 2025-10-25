export interface ILocation {
  Id: number;
  Latitude: string;
  Longtitude: string;
}

export class Location implements ILocation {
  Id: number;
  Latitude: string;
  Longtitude: string;

  constructor(data: ILocation) {
    this.Id = data.Id;
    this.Latitude = data.Latitude;
    this.Longtitude = data.Longtitude;
  }
}

