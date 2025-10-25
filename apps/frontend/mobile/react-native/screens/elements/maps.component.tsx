import React, { Component } from "react"
import MapView, { Marker, PROVIDER_GOOGLE } from "react-native-maps";
import { View, Image } from "react-native";
import { CustomElementStyles } from "./resources/styles";

const info = [
    {
        title: "Beeldende kunst",
        lat: 50.944789,
        lng: 3.122440
    },
    {
        title: "Wees gegroet",
        lat: 50.950228,
        lng: 3.142707
    },
    {
        title: "Jules Plastique",
        lat: 50.944631,
        lng: 3.1218782
    }
]

export class MapComponent extends Component<any> {
    render() {
        return (
            <MapView
                style={CustomElementStyles["maps-component-map-view"]}
                provider={PROVIDER_GOOGLE} // when you didnt add this line, you get apple maps
                initialRegion={this.props.initialRegion}
                onRegionChange={(region) => console.log(`Region: ${region}`)}
                onUserLocationChange={(location) => this.props.onUserLocationChange(location)}
                showsUserLocation={true}
            >
                {info.map((obj, key) => <Marker key={key} title={obj.title} coordinate={{ latitude: obj.lat, longitude: obj.lng }}>
                    <View>
                        <Image source={require('./resources/images/gymlogo.png')} style={CustomElementStyles["maps-component-marker"]} resizeMode="contain" />
                    </View>
                </Marker>)}

            </MapView>
        )
    }
}
