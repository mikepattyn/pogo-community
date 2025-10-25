import axios, { AxiosRequestConfig, Method, AxiosResponse, AxiosError } from "axios"
import { isNullOrUndefined } from "util";
import { inject, injectable } from "inversify";
import { Logger } from "../logger";

export const requestInterceptor = async (config: AxiosRequestConfig) => {
    if (!isNullOrUndefined(config) && !isNullOrUndefined(config.headers) && !isNullOrUndefined(config.headers.common)) {
        config.headers["Accept"] = "application/json";
        config.headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1pa2VAZ2l2dGFwcC5uZXQiLCJpYXQiOjE1ODExMDEyMTQsImV4cCI6MTU4MTE0NDQxNH0.XK69HvgoPJoXjYPZftjjcUCN1EzHK_KFr2myUC8r3s0";
    }
    return Promise.resolve(config);
}
export const errorInterceptor = async (error: AxiosError) => {
    return Promise.reject(error);
}
@injectable()
export class ApiClient {

    private baseUrl: string | undefined = "http://localhost:3000/api/v1";

    constructor(@inject(Logger) private logger: Logger) {
        // Add a request interceptor
        axios.interceptors.request.use(requestInterceptor, errorInterceptor);
    }

    async post(url: string, body: any): Promise<AxiosResponse> {
        var retVal: AxiosResponse = { data: null, status: -1, statusText: "UhOh", headers: null, config: {} }

        const config: AxiosRequestConfig = {}

        if (!isNullOrUndefined(url))
            config.url = `${this.baseUrl}${url}`
        if (!isNullOrUndefined(body))
            config.data = body;

        await axios.post(config.url!, config.data)
            .then((response: AxiosResponse) => {
                console.log(response)
                retVal = response
            })
            .catch((error: AxiosError) => {
                console.log(error)
            })
        return retVal;
    }
}

