import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { ProfileScreen } from '../components/profile/profile.screen';
import React from 'react';
import GymScreenPresentationalComponent from '../components/maps/gyms/gym.screen.presentational.component';

export type TabParamList = {
  Profile: undefined;
  Gyms: undefined;
};

const Tab = createBottomTabNavigator<TabParamList>();

export const DashboardTabs = () => {
  return (
    <Tab.Navigator
      initialRouteName="Profile"
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarStyle: {
          backgroundColor: '#232424',
          borderTopColor: '#474848',
        },
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: any;
          if (route.name === 'Profile') {
            iconName = focused ? 'account-circle' : 'account-circle-outline';
          } else if (route.name === 'Gyms') {
            iconName = 'map-marker';
          }
          return (
            <MaterialCommunityIcons name={iconName} size={size} color={color} />
          );
        },
      })}
    >
      <Tab.Screen name="Profile" component={ProfileScreen} />
      <Tab.Screen name="Gyms" component={GymScreenPresentationalComponent} />
    </Tab.Navigator>
  );
};
