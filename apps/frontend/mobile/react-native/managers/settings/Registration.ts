import { SettingsInterface } from "./SettingsInterface";
import { IRegistrationSettings } from "./IRegistrationSettings";
export class Registration extends SettingsInterface {
    private _settings: IRegistrationSettings;
    constructor() {
        super()
    }
    set settings(settings: IRegistrationSettings) {
        this._settings = settings;
    }
    get settings() {
        return this._settings;
    }
    async save() {
        this.storeData("RegistrationSettings", JSON.stringify(this._settings));
    }
}
