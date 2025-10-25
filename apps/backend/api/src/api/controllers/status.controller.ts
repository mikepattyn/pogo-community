import { Request, Response } from 'express';
import {
  BaseHttpController,
  controller,
  httpGet,
} from 'inversify-express-utils';
import moment from 'moment';

@controller('/status')
export class StatusController extends BaseHttpController {
  constructor() {
    super();
  }

  @httpGet('/')
  private health(req: Request, resp: Response) {
    var duration = process.uptime();
    var display = moment().milliseconds(duration).format('mm:ss');
    resp.json({ Uptime: display });
  }
}
