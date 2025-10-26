export interface ILocation {
  Id: number;
  Latitude: number; // DECIMAL(10, 8) in MySQL
  Longtitude: number; // DECIMAL(11, 8) in MySQL
  Name: string | null;
  Address: string | null;
  CreatedAt: Date;
}

export class Location implements ILocation {
  Id: number;
  Latitude: number;
  Longtitude: number;
  Name: string | null;
  Address: string | null;
  CreatedAt: Date;

  constructor(data: ILocation) {
    this.Id = data.Id;
    this.Latitude = data.Latitude;
    this.Longtitude = data.Longtitude;
    this.Name = data.Name;
    this.Address = data.Address;
    this.CreatedAt = data.CreatedAt;
  }
}
