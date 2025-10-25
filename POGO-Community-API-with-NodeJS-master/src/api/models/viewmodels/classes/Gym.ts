import { IGym } from "../interfaces/IGym";
import { ILocation } from "../interfaces/ILocation";

export class Gym implements IGym {
    Name: string;
    Location: ILocation;

    constructor(gym: IGym) {
        this.Name = gym.Name;
        this.Location = gym.Location;
    }
}