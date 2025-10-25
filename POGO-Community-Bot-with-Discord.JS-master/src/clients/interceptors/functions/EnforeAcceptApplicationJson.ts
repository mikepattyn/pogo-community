import { AxiosRequestConfig } from "axios";
export function EnforeAcceptApplicationJson(config: AxiosRequestConfig) {
    config.headers ? config.headers["Accept"] = "application/json" : config.headers = { Accept: "applicaton/json" }
    return Promise.resolve(config)
}