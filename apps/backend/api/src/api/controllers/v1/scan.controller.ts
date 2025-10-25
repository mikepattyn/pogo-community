import * as express from "express";
import { controller, BaseHttpController, interfaces, httpPost, requestParam, response, request } from "inversify-express-utils";
import { inject } from "inversify";
import { GoogleCloudClient } from "../../services/google/google-cloud-vision.client";
import { isNullOrUndefined } from "util";


@controller('/api/v1/scans')
export class scanController extends BaseHttpController implements interfaces.Controller {
    constructor(@inject(GoogleCloudClient) private googleCloudClient: GoogleCloudClient) { super() }

    @httpPost("/")
    private async scanImage(@request() req: express.Request, @response() res: express.Response) {
        if (req.body.url) {
            var results = await this.googleCloudClient.readImage(req.body.url)
            if (!isNullOrUndefined(results)) {
                res.status(201).json({ textResults: results })
            } else {
                res.status(400).json({ error: CustomErrors.UNDEFINED_OR_EMPTY_RESULTS })
            }
        } else {
            res.status(400).json({ error: CustomErrors.UNDEFINED_OR_EMPTY_URL })
        }
    }
}

export enum CustomErrors {
    UNDEFINED_OR_EMPTY_URL,
    UNDEFINED_OR_EMPTY_RESULTS
}