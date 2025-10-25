import React from "react";
import { View, Image, TouchableOpacity } from "react-native";
import { GymScreenContainerComponent, GymScreenState } from "./gym.screen.container.component";
import { GymScreenStyles } from "./resources/styles";
import { InputDialog } from "./../../../elements/input.dialog";

class GymScreenPresentationalComponent extends GymScreenContainerComponent<any, GymScreenState> {
    constructor(props) {
        super(props)
        this.state = {
            isDialogVisible: false,
            currentDialogValue: "",
            userLocation: null,
            gymName: ""
        }
        this.onChangeDialogInput = this.onChangeDialogInput.bind(this)
        this.onPressOutsideDialog = this.onPressOutsideDialog.bind(this)
        this.onSubmitDialog = this.onSubmitDialog.bind(this)
    }

    render() {
        return (
            <View style={GymScreenStyles.container}>
                <View style={GymScreenStyles.sidebar}>
                    <TouchableOpacity activeOpacity={0.5} onPress={() => this.showDialog(true)}>
                        <Image
                            source={require('./resources/images/gymplus.png')}
                            style={GymScreenStyles["add-gym-button"]}
                        />
                    </TouchableOpacity>
                </View>
                <InputDialog isVisible={this.state.isDialogVisible} onPressOutside={this.onPressOutsideDialog} onChangeDialogInput={this.onChangeDialogInput} onSubmitDialog={this.onSubmitDialog} />
                {this.map}
            </View>
        );
    }
}

export default GymScreenPresentationalComponent