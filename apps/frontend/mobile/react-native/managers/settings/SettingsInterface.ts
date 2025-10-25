import { AsyncStorage } from 'react-native';
export class SettingsInterface {
    async storeData(key: string, data: string) {
        try {
            await AsyncStorage.setItem(key, data);
        }
        catch (e) {
            console.log(e);
        }
    }
    async retreiveData(key: string) {
        try {
            const value = await AsyncStorage.getItem(key);
            if (value !== null) {
                return value;
            }
            else {
                return null;
            }
        }
        catch (e) {
            console.log(e);
        }
    }
}
