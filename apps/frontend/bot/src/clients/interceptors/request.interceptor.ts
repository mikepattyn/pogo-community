import { AxiosRequestConfig } from "axios";
import { EnforeAcceptApplicationJson } from "./functions/EnforeAcceptApplicationJson";

export const RequestInterceptor = async (config: AxiosRequestConfig) => {
    return await EnforeAcceptApplicationJson(config)
}