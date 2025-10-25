import { NavigationContainer } from "@react-navigation/native";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { MaterialCommunityIcons } from "@expo/vector-icons";
import { ProfileScreen } from "../components/profile/profile.screen";
import React from "react";
import GymScreenPresentationalComponent from "../components/maps/gyms/gym.screen.presentational.component";
const DashboardTabNavigator = createBottomTabNavigator()
export const DashboardTabs = () => {
    return (
        <NavigationContainer>
            <DashboardTabNavigator.Navigator
                initialRouteName="Feed"
                tabBarOptions={{
                    style: {
                        backgroundColor: "#232424",
                        borderTopColor: "#474848"
                    }
                }}
                screenOptions={({ route }) => ({
                    tabBarIcon: ({ focused, color, size }) => {
                        let iconName;
                        if (route.name === 'Profile') {
                            iconName = focused ? 'account-circle' : 'account-circle-outline'
                        } else if (route.name === 'Gyms') {
                            iconName = 'map-marker';
                        }
                        return <MaterialCommunityIcons name={iconName} size={size} color={color} />;
                    }
                })}
            >
                <DashboardTabNavigator.Screen
                    name="Profile"
                    component={ProfileScreen}
                />
                <DashboardTabNavigator.Screen
                    name="Gyms"
                    component={GymScreenPresentationalComponent}
                />
            </DashboardTabNavigator.Navigator>
        </NavigationContainer>
    )
}