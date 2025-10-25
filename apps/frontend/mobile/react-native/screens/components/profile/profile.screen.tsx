import React, { Component } from "react"
import { View, Text } from "react-native";
export class ProfileScreen extends Component {
    render() {
        return (
            <View style={{ flex: 1, justifyContent: "center", alignItems: "center", backgroundColor: "#232424" }}>
                <Text>Profile</Text>
            </View>
        );
    }
}