import React, { Component } from "react";
import { View, Image, TouchableHighlight } from "react-native";

export class HeaderSubcomponent extends Component<any, any> {

    render() {
        return (
            <View style={{ justifyContent: "center", alignItems: "flex-start", height: 70, width: "100%" }} >
                <TouchableHighlight onPress={() => this.props.navigation.navigate()}>
                    <Image style={{ marginLeft: 10, height: 50, width: 50 }} source={require('./poke-ball.png')} />
                </TouchableHighlight >
            </View>
        )
    }
}