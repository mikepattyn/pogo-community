import { IDataLocation } from "../interfaces/IDataLocation";

export class DataLocation implements IDataLocation {
    Id: number;
    Latitude: string;
    Longtitude: string

    constructor(dataLocation: IDataLocation) {
        this.Id = dataLocation.Id;
        this.Latitude = dataLocation.Latitude;
        this.Longtitude = dataLocation.Longtitude;
    }

    static Insert() {
        return "INSERT INTO Locations (Latitude, Longtitude) VALUES (?, ?)"
    }

    static GetById() {
        return "SELECT * FROM Locations WHERE Id= ?"
    }
}