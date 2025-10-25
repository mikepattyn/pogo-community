// file inversify.config.ts

import { Container } from 'inversify';
import { RaidService } from './services/raid.service';
import { MessageService } from './services/message.service';
import { PokemonService } from './services/pokemon.service';
import { PokemonStore } from './stores/pokemon.store';
import { RaidStore } from './stores/raid.store';
import { ApiClient } from './clients/apiClient';
import { MicroservicesClient } from './clients/microservices.client';
import { Logger } from './logger';

const dependencyInjectionContainer = new Container();
// Services
dependencyInjectionContainer
  .bind<RaidService>(RaidService)
  .toSelf()
  .inSingletonScope();
dependencyInjectionContainer
  .bind<PokemonService>(PokemonService)
  .toSelf()
  .inSingletonScope();
dependencyInjectionContainer
  .bind<MessageService>(MessageService)
  .toSelf()
  .inSingletonScope();

// Stores
dependencyInjectionContainer
  .bind<RaidStore>(RaidStore)
  .toSelf()
  .inSingletonScope();
dependencyInjectionContainer
  .bind<PokemonStore>(PokemonStore)
  .toSelf()
  .inSingletonScope();

// Clients
dependencyInjectionContainer
  .bind<ApiClient>(ApiClient)
  .toSelf()
  .inSingletonScope();

dependencyInjectionContainer
  .bind<MicroservicesClient>(MicroservicesClient)
  .toSelf()
  .inSingletonScope();

// Other
dependencyInjectionContainer
  .bind<Logger>('Logger')
  .to(Logger)
  .inSingletonScope();

export { dependencyInjectionContainer };
