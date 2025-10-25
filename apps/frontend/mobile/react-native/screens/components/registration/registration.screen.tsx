import * as React from "react";
import { View, Text, ImageBackground } from "react-native";
import validator from "email-validator"
import { RegistrationInput } from "../../elements/registration.input";
import { RegistrationState } from "./RegistrationState";
import { RegistrationProps } from "./RegistrationProps";
import { Button } from "react-native-elements";

export class RegistrationScreen extends React.Component<any, RegistrationState> {
    private propertyCommands: string[] = ["Enter your email address", "Enter your password", "Re-enter your password", "Enter your Discord id", "Enter your first name", "Enter your nickname", "Enter your level", "Select your team"]
    constructor(props: RegistrationProps) {
        super(props);
        this.state = {
            index: 0,
            currentValue: "",
            currentButtonValue: "Next",
            currentNameValue: "Email",
            Error: ""
        }
        this.onChangeText = this.onChangeText.bind(this);
    }
    render() {
        console.log(this.props)
        return (
            <ImageBackground source={require('./../../../resources/images/home_background.png')} imageStyle={{ resizeMode: "contain" }} style={{ width: '100%', height: '100%', backgroundColor: "#232424" }}>
                <View style={{ flex: 1 }}></View>
                <View style={{ flex: 1, alignItems: "center", width: "100%", padding: 20 }}>
                    <View style={{ flex: 1, justifyContent: "space-evenly", width: "100%" }}>
                        <Text style={{ fontSize: 20, color: "#e1e1e1", textAlign: "center" }}>{this.propertyCommands[this.state.index]}</Text>
                        <RegistrationInput index={this.state.index} onChange={(text: string) => this.onChangeText(text)} value={this.state.currentValue} name={this.state.currentNameValue} />
                        <Text style={{ fontSize: 14, color: "red", textAlign: "center" }}>{this.state.Error}</Text>
                    </View>
                </View>
                <Button containerStyle={{ width: "100%", paddingHorizontal: 20 }} buttonStyle={{ backgroundColor: "#2164E8", marginBottom: 20 }} title={this.state.currentButtonValue} onPress={() => this.onSubmit()} disabled={this.state.Error != undefined} />
            </ImageBackground>
        )
    }
    onChangeText(text: string) {
        this.setState((prevState) => ({
            ...prevState,
            currentValue: text
        }))
        // Email
        if (this.state.index == 0) {
            if (text.length > 7 && validator.validate(text)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Email: text,
                    Error: undefined
                }))
            } else if (text.length > 7 && !validator.validate(text)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Incorrect email."
                }))
            } else if (text.length <= 7) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }

        }

        // Password
        if (this.state.index == 1) {
            var regex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$")
            if (text.length >= 8 && regex.test(text)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Password: text,
                    Error: undefined
                }))
            } else if (text.length >= 8 && !regex.test(text)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Wrong password"
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }
        }

        // Re-Enter password
        if (this.state.index == 2) {
            var match = text == this.state.Password
            if (match) {
                this.setState((prevState) => ({
                    ...prevState,
                    Password: text,
                    Error: undefined
                }))
            } else if (text.length >= 8 && !match) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Wrong password"
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }
        }

        // Discord id
        if (this.state.index == 3) {
            if (text.length >= 8) {
                this.setState((prevState) => ({
                    ...prevState,
                    DiscordId: text,
                    Error: undefined
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }
        }

        // Firstname
        if (this.state.index == 4) {
            if (text.length >= 2) {
                this.setState((prevState) => ({
                    ...prevState,
                    FirstName: text,
                    Error: undefined
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }
        }

        // Nickname
        if (this.state.index == 5) {
            if (text.length >= 2) {
                this.setState((prevState) => ({
                    ...prevState,
                    Nickname: text,
                    Error: undefined
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: ""
                }))
            }
        }

        // Level
        if (this.state.index == 6) {
            var level = Number(text)
            if (isNaN(level)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Please enter a number"
                }))
            } else if (!isNaN(level) && level < 1 || level > 40) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Please enter a number between 1 and 40"
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Level: level,
                    Error: undefined
                }))
            }
        }

        // Team
        if (this.state.index == 7) {
            var team = Number(text)
            if (isNaN(team)) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Please select a team"
                }))
            } else if (!isNaN(team) && team < 0 || team > 2) {
                this.setState((prevState) => ({
                    ...prevState,
                    Error: "Please select a team"
                    // this shouldnt happen above
                }))
            } else {
                this.setState((prevState) => ({
                    ...prevState,
                    Team: team,
                    Error: undefined,
                    currentButtonValue: "Finish"
                }))
            }
        }
    }
    async onSubmit() {
        if (this.state.index == 0) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Password"
            }))
        }
        else if (this.state.index == 1) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Re-enter password"
            }))
        }
        else if (this.state.index == 2) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Discord id"
            }))
        }
        else if (this.state.index == 3) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Firstname"
            }))
        }
        else if (this.state.index == 4) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Nickname"
            }))
        }
        else if (this.state.index == 5) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Level"
            }))
        }
        else if (this.state.index == 6) {
            this.setState((prevState) => ({
                ...prevState,
                currentNameValue: "Team"
            }))
        }

        if (this.state.index < 7) {
            this.setState((prevState) => ({
                ...prevState,
                index: this.state.index + 1,
                currentValue: "",
                Error: ""
            }))
        } else if (this.state.index == 7) {
            this.props.navigation.navigate('Dashboard')
        }

    }

}
