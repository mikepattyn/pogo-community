import React from "react";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { AppDrawerNavigator } from "./app.drawer.navigator";
import { RegistrationScreen } from "../components/registration/registration.screen";
import { WelcomeScreen } from "../components/welcome/welcome.screen";

export type RootStackParamList = {
    Welcome: undefined;
    Dashboard: undefined;
    Registration: undefined;
};

const Stack = createNativeStackNavigator<RootStackParamList>();

export const AppSwitchNavigator = () => {
    return (
        <Stack.Navigator 
            initialRouteName="Welcome"
            screenOptions={{ headerShown: false }}
        >
            <Stack.Screen name="Welcome" component={WelcomeScreen} />
            <Stack.Screen name="Dashboard" component={AppDrawerNavigator} />
            <Stack.Screen name="Registration" component={RegistrationScreen} />
        </Stack.Navigator>
    );
};
