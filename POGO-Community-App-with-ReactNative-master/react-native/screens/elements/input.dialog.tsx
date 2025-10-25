import React, { Component } from "react";
import { Keyboard, View, StyleSheet } from "react-native";
import { Input } from "react-native-elements";
import { Dialog, DialogContent, DialogTitle, DialogFooter, DialogButton } from 'react-native-popup-dialog';
import ApiClient from "../../clients/apiClient";
import GymManager from "../../managers/gym.manager";

export interface InputDialogProps { 
    onChangeDialogInput(): void;
    dialogTitle: string;
    dialogSubmitText: string;
    dialogCancelText: string;
    dialogSubmit: Function;
    dialogCancel: Function;
    isDialogVisible: boolean
}

// TODO: Test the custom InputDialog - Check if state is maintained and correctly passed to and from the child component;

export class InputDialog extends Component<any, any> {
    render() {
        return (
            <Dialog
            visible={this.props.isVisible}
            onTouchOutside={() => this.props.onPressOutside()}
            dialogTitle={<DialogTitle title="Enter gymname" />}
            footer={
                <DialogFooter>
                    <DialogButton
                        text="SUBMIT"
                        onPress={() => this.props.onSubmitDialog()}
                    />
                    <DialogButton
                        text="CANCEL"
                        onPress={() => this.props.onPressOutside()}
                    />
                </DialogFooter>
            }
            width={0.8}
        >
            <DialogContent>
                <View style={styles.dialogContentContainer}>
                    <Input 
                        inputStyle={styles.dialogInput} 
                        value={this.props.currentDialogValue} 
                        placeholder="Gymname" 
                        onChangeText={(text) => this.props.onChangeDialogInput(text)} 
                    />
                </View >
            </DialogContent>
        </Dialog>
        )
    }
}

const styles = StyleSheet.create({
    dialogContentContainer: { width: "100%", height: 30, display: "flex", marginTop: 10, alignContent: "center", justifyContent: "center" },
    dialogInput: { color: "#e1e1e1", textAlign: "left", textAlignVertical: "center" }
 })
