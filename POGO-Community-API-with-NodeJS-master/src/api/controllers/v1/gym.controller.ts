import * as express from "express"
import { controller, httpGet, interfaces, requestParam, response, httpPost, request, requestHeaders, BaseHttpController } from "inversify-express-utils"
import { inject } from "inversify";
import { GymStore } from "../../stores/gym.store";
import { isNullOrUndefined } from "util";

@controller('/api/v1/gyms')
export class gymController extends BaseHttpController implements interfaces.Controller {
    constructor(@inject(GymStore) private gymStore: GymStore) { super() }

    @httpGet('/:id')
    private async getById(@requestParam("id") id: number, @response() res: express.Response) {
        if (await this.httpContext.user.isAuthenticated()) {
            await this.gymStore.getById(id).then(result => {
                res.status(200).json(result)
            }).catch((error: any) => {
                res.status(500).json(error)
            })
        } else {
            res.status(401).end()
        }
    }

    @httpPost('/')
    private async post(@request() req: express.Request, @response() res: express.Response) {
        if (await this.httpContext.user.isAuthenticated()) {
            try {
                var insertedId = await this.gymStore.post(req.body)
                if (!isNullOrUndefined(insertedId) && !isNaN(Number(insertedId)))
                    res.status(201).json({ id: insertedId })
                else
                    res.status(400).json("Something unexpcted went wrong. this shuldnt happeeen")
            } catch (error) {
                res.status(400).json(error)
            }

        } else {
            res.status(401).end()
        }
    }

    @httpGet('/')
    private async getAll(@request() req: express.Request, @response() res: express.Response) {
        if (await this.httpContext.user.isAuthenticated()) {
            try {
                var results = await this.gymStore.getAll();
                console.log("Results from get all gyms: ", results)
                if (!isNullOrUndefined(results))
                    res.status(200).json({ gyms: results })
                else
                    res.status(400).json("Something unexpcted went wrong. this shuldnt happeeen")
            } catch (error) {
                res.status(400).json(error)
            }
        } else {
            res.status(401).end()
        }
    }
}