import * as express from 'express';
import {
  interfaces,
  controller,
  httpPost,
  request,
  response,
  BaseHttpController,
  httpGet,
  queryParam,
} from 'inversify-express-utils';
import { inject } from 'inversify';
import { PlayerStore } from '../../stores/player.store';

@controller('/api/v1/players')
export class playerController
  extends BaseHttpController
  implements interfaces.Controller
{
  constructor(@inject(PlayerStore) private playerStore: PlayerStore) {
    super();
  }

  @httpPost('/')
  private async post(
    @request() req: express.Request,
    @response() res: express.Response
  ) {
    if (await this.httpContext.user.isAuthenticated()) {
      await this.playerStore
        .post(req.body)
        .then(() => {
          res.sendStatus(201);
        })
        .catch((error: any) => {
          res.status(400).json(error);
        });
    } else {
      res.status(401).end();
    }
  }

  @httpGet('/')
  private async get(
    @queryParam('DiscordId') id: number,
    @response() res: express.Response
  ) {
    if (await this.httpContext.user.isAuthenticated()) {
      await this.playerStore
        .get(id)
        .then((response) => {
          res.status(200).json(response);
        })
        .catch((error: any) => {
          res.status(400).json(error);
        });
    } else {
      res.status(401).end();
    }
  }
}
