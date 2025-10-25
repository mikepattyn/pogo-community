import * as express from 'express';
import {
  controller,
  request,
  response,
  httpPost,
  httpGet,
  requestParam,
  interfaces,
} from 'inversify-express-utils';
import { inject } from 'inversify';
import passwordHash from 'password-hash';
import validator from 'email-validator';

import { AuthStore } from '../../stores/account.store';
import { isNullOrUndefined } from 'util';
import { Logger } from '../../logger';
var jwt = require('jsonwebtoken');

@controller('/api/v1/auth')
export class authController implements interfaces.Controller {
  constructor(
    @inject(AuthStore) private authStore: AuthStore,
    @inject(Logger) private logger: Logger
  ) {}

  @httpPost('/users')
  private async register(
    @request() req: express.Request,
    @response() res: express.Response
  ) {
    var password = req.body.Password;
    // Minimum eight characters, at least one uppercase letter, one lowercase letter and one number:
    if (
      !new RegExp('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$').test(
        password
      )
    ) {
      res
        .status(400)
        .send(
          'Password has to be minimum eight characters, at least one uppercase letter, one lowercase letter and one number'
        );
      return;
    }

    // Validate email
    if (!validator.validate(req.body.Email)) {
      res.status(400).send('Wrong email. Please check it again.');
      return;
    }

    // Generate password hash
    try {
      var hashedPassword = passwordHash.generate(password);
      // create body with password hash
      var body = {
        Email: req.body.Email,
        PlayerId: req.body.PlayerId,
        Password: hashedPassword,
        DateJoined: req.body.DateJoined,
      };

      await this.authStore
        .post(body)
        .then(async () => {
          this.logger.log(`User registered with email: ${body.Email}`);
          res.sendStatus(201);
        })
        .catch((error: any) => {
          res.status(400).json(error);
        });
    } catch (e) {
      res.status(500).send(e);
    }
  }

  @httpPost('/users/login')
  private async login(
    @request() req: express.Request,
    @response() res: express.Response
  ) {
    const password = req.body.Password;

    // validate password
    if (
      !new RegExp('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$').test(
        password
      )
    ) {
      res
        .status(400)
        .send(
          'Password has to be minimum eight characters, at least one uppercase letter, one lowercase letter and one number'
        );
      return;
    }

    // validate email
    if (!validator.validate(req.body.Email)) {
      res.status(400).send('Wrong email. Please check it again.');
      return;
    }

    // get account by email adress
    const databaseAccount = await this.authStore.getByEmail(req.body.Email);

    // check if account exists
    if (isNullOrUndefined(databaseAccount)) {
      res.status(400).send('Cannot find an account with this name.');
      return;
    }

    // check if passwords match
    if (!passwordHash.verify(password, databaseAccount.Password)) {
      res.status(400).send('Wrong password. Please try again');
      return;
    }

    // sign token
    const token = await jwt.sign(
      { email: req.body.Email },
      process.env.JWT_KEY,
      { expiresIn: '12h' }
    );
    if (isNullOrUndefined(token)) {
      res
        .status(500)
        .send('Something internal went wrong. Please contact admin.');
      return;
    }

    // return token
    this.logger.log(`User logged in: ${req.body.Email}`);
    res.status(200).send(token);
    return;
  }
}
