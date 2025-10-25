import * as express from "express"
import { injectable } from "inversify";
import { interfaces } from "inversify-express-utils";
import { AuthService } from "./auth.service";
import { isNullOrUndefined } from "util";

@injectable()
export class CustomAuthProvider implements interfaces.AuthProvider {
    public async getUser(
        req: express.Request,
        res: express.Response,
        next: express.NextFunction
    ): Promise<interfaces.Principal> {
        console.log(req.headers)
        var token = req.headers["authorization"]
        if (!isNullOrUndefined(token)) {
            token = token.replace("Bearer ", "")
            const user = AuthService.VerifyToken(token);
            const principal = new Principal(user);
            return principal;
        }
        else {
            return new Principal(null)
        }
    }
}

class Principal implements interfaces.Principal {
    public details: any;
    public constructor(details: any) {
        this.details = details;
    }
    public isAuthenticated(): Promise<boolean> {
        return Promise.resolve(!isNullOrUndefined(this.details));
    }
    public isResourceOwner(resourceId: any): Promise<boolean> {
        return Promise.resolve(resourceId === 1111);
    }
    public isInRole(role: string): Promise<boolean> {
        return Promise.resolve(role === "admin");
    }
}