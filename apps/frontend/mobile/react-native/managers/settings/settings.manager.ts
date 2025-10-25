import { Registration } from './Registration';

export class SettingsManager {
    private _registration: Registration
    set registration(registration: Registration) { this._registration = registration };
    get registration() { return this._registration }
    constructor() {

    }
}


