import { IDataGym } from '../interfaces/IDataGym';

export class DataGym implements IDataGym {
  Id: number;
  Name: string;
  LocationId: number;

  constructor(dataGym: IDataGym) {
    this.Id = dataGym.Id;
    this.Name = dataGym.Name;
    this.LocationId = dataGym.LocationId;
  }

  static Insert() {
    return 'INSERT INTO Gyms (Name, LocationId) VALUES (?, ?)';
  }

  static GetById() {
    return 'SELECT * FROM Gyms WHERE Id= ?';
  }

  static GetAllWithLocations() {
    return `
            SELECT Gyms.Name, Locations.Latitude, Locations.Longtitude FROM Gyms 
                INNER JOIN Locations ON Gyms.LocationId = Locations.Id
            WHERE NOT Gyms.Name = '' 
        `;
  }
}
