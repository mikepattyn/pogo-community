import { LatLng } from "react-native-maps";

export interface IGymManager {
    addGym(name: string, position: LatLng): void
    getGyms(): GymView[]
}

export class GymView { 
    name: string
    location: LatLng
}