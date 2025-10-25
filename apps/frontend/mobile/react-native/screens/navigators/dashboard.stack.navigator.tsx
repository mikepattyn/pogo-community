import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { DashboardTabs } from './dashboard.tabs';

export type DashboardStackParamList = {
  DashboardTabNavigator: undefined;
};

const Stack = createNativeStackNavigator<DashboardStackParamList>();

export const DashboardStackNavigator = () => {
  return (
    <Stack.Navigator>
      <Stack.Screen
        name="DashboardTabNavigator"
        component={DashboardTabs}
        options={{
          title: 'POGO Community Raid Bot',
          headerStyle: {
            backgroundColor: '#232424',
          },
          headerTitleStyle: {
            color: '#e1e1e1',
          },
          headerTintColor: '#e1e1e1',
        }}
      />
    </Stack.Navigator>
  );
};
