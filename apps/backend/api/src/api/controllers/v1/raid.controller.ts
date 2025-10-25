import * as express from 'express';
import {
  controller,
  httpGet,
  interfaces,
  requestParam,
  response,
  httpPost,
  request,
  BaseHttpController,
} from 'inversify-express-utils';
import { inject } from 'inversify';
import { RaidStore } from '../../stores/raid.store';
import { RaidViewModel } from '../../models/viewmodels/RaidViewModel';
import { Location } from '../../models/viewmodels/classes/Location';
import { ILocation } from '../../models/viewmodels/interfaces/ILocation';
import { IGym } from '../../models/viewmodels/interfaces/IGym';
import { Gym } from '../../models/viewmodels/classes/Gym';
import { Raid } from '../../models/viewmodels/classes/Raid';

@controller('/api/v1/raids')
export class raidController
  extends BaseHttpController
  implements interfaces.Controller
{
  constructor(@inject(RaidStore) private raidStore: RaidStore) {
    super();
  }

  @httpGet('/:id')
  private async getByMessageId(
    @requestParam('id') id: string,
    @response() res: express.Response
  ) {
    if (await this.httpContext.user.isAuthenticated()) {
      await this.raidStore
        .getByMessageId(id)
        .then((result) => {
          console.log(result[0]);
          const location: ILocation = new Location({
            Longtitude: result[0].Longtitude,
            Latitude: result[0].Latitude,
          });
          const gym: IGym = new Gym({ Name: result[0].Name, Location: location });
          const raid: Raid = new Raid(
            result[0].Pokemon,
            gym,
            result[0].Tiers,
            result[0].TimeRemaining
          );

          // return view result.
          res.status(200).json(new RaidViewModel(raid));
        })
        .catch((error: unknown) => {
          res.status(400).json(error);
        });
    } else {
      res.status(401).end();
    }
  }

  @httpPost('/')
  private async post(
    @request() req: express.Request,
    @response() res: express.Response
  ) {
    if (await this.httpContext.user.isAuthenticated()) {
      await this.raidStore
        .post(req.body)
        .then(() => {
          res.sendStatus(201);
        })
        .catch((error: unknown) => {
          res.status(400).json(error);
        });
    } else {
      res.status(401).end();
    }
  }

  @httpPost('/:id/players')
  private async addPlayer(
    @request() req: express.Request,
    @response() res: express.Response
  ) {
    if (await this.httpContext.user.isAuthenticated()) {
      await this.raidStore
        .addPlayer(req.params.id, req.body.PlayerId)
        .then(() => {
          res.sendStatus(201);
        })
        .catch((error: unknown) => {
          res.status(400).json(error);
        });
    } else {
      res.status(401).end();
    }
  }
}
