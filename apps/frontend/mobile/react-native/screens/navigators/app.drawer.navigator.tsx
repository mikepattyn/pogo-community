import { createDrawerNavigator } from "react-navigation-drawer";
import { DashboardStackNavigator } from "./dashboard.stack.navigator";

export const AppDrawerNavigator = createDrawerNavigator({
    Dashboard: {
        screen: DashboardStackNavigator
    }
})