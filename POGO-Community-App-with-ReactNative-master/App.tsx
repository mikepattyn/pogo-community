import { createAppContainer, } from "react-navigation";
import { AppSwitchNavigator } from "./react-native/screens/navigators/app.switch.navigator";
import React from "react";

const AppContainer = createAppContainer(AppSwitchNavigator);
const App = (props) => (<AppContainer />)

export default App 