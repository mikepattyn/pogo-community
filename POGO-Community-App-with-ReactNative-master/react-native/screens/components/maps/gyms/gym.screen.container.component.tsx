import React, { Component } from "react";
import { View, Keyboard } from "react-native";
import { isNullOrUndefined } from "util";
import { MapComponent } from "../../../elements/maps.component";
import GymManager from "../../../../managers/gym.manager";
import { LatLng } from "react-native-maps";

export interface GymScreenState {
    userLocation: LatLng
    gymName: string
    currentDialogValue: string
    isDialogVisible: boolean
}

export class GymScreenContainerComponent<P, S> extends Component<any, GymScreenState> {
    private gymManager: GymManager = GymManager.instance
    constructor(props) {
        super(props);
        this.onUserLocationChange = this.onUserLocationChange.bind(this)
    }
    map: any
    // _mapView: MapView
    componentWillReceiveProps() {
        this.getCurrentPosition()
        this.initMap()
        this.forceUpdate();
    }
    componentDidMount() {
        this.getCurrentPosition()
        this.initMap()
        this.forceUpdate();
    }
    initMap() {
        var location = {
            latitude: 50.950228,
            longitude: 3.142707
        }
        if (!isNullOrUndefined(this.state.userLocation)) {
            console.log("found location")
            location = this.state.userLocation
        }
        // this.map = <MapComponent location={location} mapElement={<div style={{ flex: 1, height: "100%", width: "100%" }} />} containerElement={<div style={{ flex: 1, height: "100%", width: "100%" }} />}></MapComponent>
        this.map = (
            <MapComponent initialRegion={location} onUserLocationChange={this.onUserLocationChange} />
        )
    }

    showGymDialog() {
        console.log('show gym dialog called')
        this.setState((prevState) => ({
            ...prevState,
            isDialogVisible: true
        }))
        this.forceUpdate();
    }
    onChangeDialogInput(text: string) {
        this.setState((prevState) => ({
            ...prevState,
            currentDialogValue: text
        }))
    }
    async onSubmitDialog() {
        this.setState((prevState) => ({
            ...prevState,
            gymName: this.state.currentDialogValue,
            currentDialogValue: ""
        }))
        console.log(`Response: ${await this.gymManager.addGym(this.state.gymName, this.state.userLocation)}`)
    }
    onPressOutsideDialog() {
        this.setState((prevState) => ({
            ...prevState,
            isDialogVisible: false
        }))
        Keyboard.dismiss()
    }
    showDialog(show: boolean) {
        this.setState((prevState) => ({
            ...prevState,
            isDialogVisible: show
        }))
    }
    onUserLocationChange(location) {
        var coordinate = location.nativeEvent.coordinate
        this.setState((prevState) => ({
            ...prevState,
            userLocation: { latitude: coordinate.latitude, longitude: coordinate.longitude, latitudeDelta: 0.04, longitudeDelta: 0.04 },
        }))
    }
    getCurrentPosition() {
        if (this.weCanGetGeolocation()) {
            return navigator.geolocation.getCurrentPosition(
                (position: Position) => {
                    const { latitude, longitude } = position.coords;

                    this.setState((prevState) => ({
                        ...prevState,
                        userLocation: { latitude: latitude, longitude: longitude },
                        locationLoading: false
                    }))
                },
                (error: PositionError) => {
                    this.setState((prevState) => ({
                        ...prevState,
                        locationLoading: false
                    }))
                    console.log(error)
                    console.log("this should not happen ... ")
                }
            )
        }
    }

    weCanGetGeolocation() {
        return !isNullOrUndefined(navigator.geolocation)
    }
}