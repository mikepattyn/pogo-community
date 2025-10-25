import { createSwitchNavigator } from "react-navigation";
import { AppDrawerNavigator } from "./app.drawer.navigator";
import { RegistrationScreen } from "../components/registration/registration.screen";
import { WelcomeScreen } from "../components/welcome/welcome.screen";

export const AppSwitchNavigator = createSwitchNavigator({
    Welcome: { screen: WelcomeScreen },
    Dashboard: { screen: AppDrawerNavigator },
    Registration: { screen: RegistrationScreen }
});
