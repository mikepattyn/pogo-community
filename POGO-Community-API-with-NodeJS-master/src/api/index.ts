import express from 'express'
import path from 'path'
import bodyParser from 'body-parser'
import * as pkg from '../package.json';
import "reflect-metadata"
import "./controllers/status.controller";
import "./controllers/v1/account.controller";
import "./controllers/v1/gym.controller";
import "./controllers/v1/raid.controller";
import "./controllers/v1/player.controller";
import "./controllers/v1/location.controller";
import "./controllers/v1/scan.controller";
import { Container } from 'inversify';
import { InversifyExpressServer } from 'inversify-express-utils';
import { Logger } from './logger';
import { RaidStore } from './stores/raid.store.js';
import { GymStore } from './stores/gym.store.js';
import { PlayerStore } from './stores/player.store.js';
import { LocationStore } from './stores/location.store.js';
import { AuthStore } from './stores/account.store.js';
import { CustomAuthProvider } from './services/auth/custom.auth.provider.js';
import { GoogleCloudServices } from './services/google/google-cloud.services.js';
import { GoogleCloudClient } from './services/google/google-cloud-vision.client.js';

// Give Views/Layouts direct access to session data.

require('dotenv').config()

var port = process.env.PORT || "8080"
// set up container
let container = new Container();

// set up bindings
container.bind<Logger>(Logger).to(Logger);

container.bind<RaidStore>(RaidStore).to(RaidStore).inSingletonScope();
container.bind<GymStore>(GymStore).to(GymStore).inSingletonScope();
container.bind<PlayerStore>(PlayerStore).to(PlayerStore).inSingletonScope();
container.bind<LocationStore>(LocationStore).to(LocationStore).inSingletonScope();
container.bind<AuthStore>(AuthStore).to(AuthStore).inSingletonScope();

container.bind<GoogleCloudServices>(GoogleCloudServices).to(GoogleCloudServices).inSingletonScope();
container.bind<GoogleCloudClient>(GoogleCloudClient).to(GoogleCloudClient).inSingletonScope();

// create server
let server = new InversifyExpressServer(container, null, null, null, CustomAuthProvider);
server.setConfig((app) => {
  // add body parser
  app.use(bodyParser.urlencoded({
    extended: true
  }));
  app.use(bodyParser.json());
});

let app = server.build();

app.use(function (req, res, next) {
  res.locals.version = `Build: ${(<any>pkg)["version"]}`
  next();
});

app.use(express.static(path.join(__dirname, "/../public")))

// Serve the application at the given port
app.listen(port, () => {
  // Success callback
  console.log(`Listening at http://localhost:${port}/`);
});


