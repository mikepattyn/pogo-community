import axios, { AxiosRequestConfig, AxiosResponse, AxiosError } from "axios"
import { isNullOrUndefined, isNull } from "util";
import { requestInterceptor, errorInterceptor } from "./interceptors";

export default class ApiClient {
    static myInstance = null;
    static get instance() {
        if(isNull(ApiClient.myInstance)) {
            this.myInstance = new ApiClient()
        }
        return this.myInstance
    }
    private baseUrl: string | undefined = "http://192.168.77.51:54353/api/v1";

    constructor() {
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
                retVal = response
            })
            .catch((error: AxiosError) => {
                console.log(error)
            })
        return retVal;
    }

    async get(url: string) {
        var retVal: AxiosResponse = { data: null, status: -1, statusText: "UhOh", headers: null, config: {} }

        const config: AxiosRequestConfig = {}

        if (!isNullOrUndefined(url))
            config.url = `${this.baseUrl}${url}`

        await axios.get(config.url!)
            .then((response: AxiosResponse) => {
                retVal = response
            })
            .catch((error: AxiosError) => {
                console.log(error)
            })
        return retVal;
    }
}

