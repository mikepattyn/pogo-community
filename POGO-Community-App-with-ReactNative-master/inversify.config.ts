// file inversify.config.ts
import { Container } from "inversify";
import { ApiClient } from "./react-native/clients/apiClient";
import { Logger } from "./react-native/logger";
import { Settings } from "./react-native/managers/settings/settings.manager";

const dependencyInjectionContainer = new Container();
// Cients
dependencyInjectionContainer.bind<ApiClient>(ApiClient).toSelf().inSingletonScope();

// Managers
dependencyInjectionContainer.bind<Settings>(Settings).toSelf().inSingletonScope();

// Other
dependencyInjectionContainer.bind<Logger>(Logger).toSelf().inSingletonScope();

export { dependencyInjectionContainer };