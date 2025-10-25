import React, { Component } from "react";
import { SettingsManager } from "../../../managers/settings/settings.manager";
import { ImageBackground, View, Text } from "react-native";
import { Button } from "react-native-elements";

export class WelcomeScreen extends Component<any> {

    registrationManager: SettingsManager = new SettingsManager();

    constructor(props) {
        super(props)
        // this.registrationManager.registration.storeData("Registration", JSON.stringify(this.registrationManager.registration.settings))

    }

    render() {
        console.log(this.props)
        return (
            <ImageBackground source={require('./../../../resources/images/home_background.png')} imageStyle={{ resizeMode: "contain" }} style={{ width: '100%', height: '100%', backgroundColor: "#232424" }}>
                <View style={{ flex: 1, justifyContent: "flex-end", alignItems: "center" }}>
                    <Text style={{ fontSize: 20, marginTop: 10, marginBottom: 20, color: "#e1e1e1" }}>Welcome to the Raid Bot app</Text>
                </View>
                <View style={{ flex: 1, justifyContent: "flex-end", alignItems: "center" }}>
                    <Button containerStyle={{ width: "100%", paddingHorizontal: 20 }} buttonStyle={{
                        backgroundColor: "#2164E8"
                    }} title='Login' onPress={() => this.props.navigation.navigate('Dashboard')} />
                    <Text style={{ textDecorationLine: "underline", fontSize: 12, marginTop: 10, marginBottom: 20, color: "#e1e1e1" }} numberOfLines={1} onPress={() => this.props.navigation.navigate('Registration')}>Dont have an account yet? Click here to sign up.</Text>
                </View>
            </ImageBackground>

        );
    }
}