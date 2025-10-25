import jwt from 'jsonwebtoken';
import { isNullOrUndefined } from 'util';

export class AuthService {
  static VerifyToken(token: string) {
    try {
      let retVal: string | null | object = jwt.verify(
        token.replace('Bearer ', ''),
        process.env.JWT_KEY!
      );
      if (isNullOrUndefined(retVal)) {
        retVal = null;
      }
      return retVal;
    } catch (e) {
      console.log(e);
    }
  }
}
