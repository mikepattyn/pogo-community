import { ILocation } from "../interfaces/ILocation";

export class Location implements ILocation {
    Longtitude: string;
    Latitude: string;

    constructor(location: ILocation) {
        this.Latitude = location.Latitude
        this.Longtitude = location.Longtitude
    }
}