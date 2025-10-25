import { IGymManager, GymView } from "./gym.manager.interface"
import { LatLng } from "react-native-maps";
import ApiClient from "../clients/apiClient";
import { isNull } from "util";

export default class GymManager implements IGymManager {
    static myInstance = null;
    static get instance() {
        if(isNull(GymManager.myInstance)) {
            this.myInstance = new GymManager()
        }
        return this.myInstance
    }
    async addGym(name: string, position: LatLng): Promise<void> {
        var locationResponse = await ApiClient.instance.post("/locations", { Latitude: position.latitude, Longtitude: position.longitude })
        if(locationResponse.status >= 200 && locationResponse.status < 300) {
            console.log(`Sending name: ${name} - Location: ${JSON.stringify(position)}`)
            return await ApiClient.instance.post("/gyms", { Name: name, LocationId: locationResponse.data.id })
        }
    }
    async getGyms(): Promise<GymView[]> {
        return await ApiClient.instance.get("/gyms");
    }
}

