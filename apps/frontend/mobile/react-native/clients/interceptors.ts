import { AxiosRequestConfig, AxiosError } from "axios";

import { isNullOrUndefined } from "util";

export const requestInterceptor = async (config: AxiosRequestConfig) => {
    if (!isNullOrUndefined(config) && !isNullOrUndefined(config.headers) && !isNullOrUndefined(config.headers.common)) {
        config.headers["Accept"] = "application/json";
        config.headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1pa2UrMUBnaXZ0YXBwLm5ldCIsImlhdCI6MTU4MjA5ODY0NCwiZXhwIjoxNTgyMTQxODQ0fQ.q0NmffTfGHZnspUCP86bvjZsvoXR-Uq12ulZDCM06WI";
    }
    return Promise.resolve(config);
}
export const errorInterceptor = async (error: AxiosError) => {
    return Promise.reject(error);
}