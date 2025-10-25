import { StyleSheet } from "react-native";

export const GymScreenStyles = StyleSheet.create({
    container: {
        ...StyleSheet.absoluteFillObject,
        height: "100%",
        width: "100%",
        justifyContent: 'flex-end',
        alignItems: 'center',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        position: 'absolute'
    },
    sidebar: {
        height: 300, width: 40, position: "absolute", right: 10, top: 10, zIndex: 1
    },
    "add-gym-button": {
        width: 40, height: 40
    },
    map: {
        ...StyleSheet.absoluteFillObject,
    },
});