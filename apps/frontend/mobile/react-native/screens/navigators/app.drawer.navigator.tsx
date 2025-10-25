import React from "react";
import { createDrawerNavigator } from "@react-navigation/drawer";
import { DashboardStackNavigator } from "./dashboard.stack.navigator";

export type DrawerParamList = {
    Dashboard: undefined;
};

const Drawer = createDrawerNavigator<DrawerParamList>();

export const AppDrawerNavigator = () => {
    return (
        <Drawer.Navigator>
            <Drawer.Screen 
                name="Dashboard" 
                component={DashboardStackNavigator}
                options={{ headerShown: false }}
            />
        </Drawer.Navigator>
    );
};