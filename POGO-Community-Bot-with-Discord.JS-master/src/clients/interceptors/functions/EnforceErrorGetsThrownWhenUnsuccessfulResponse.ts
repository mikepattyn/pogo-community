import { AxiosResponse } from "axios";
export function EnforceErrorGetsThrownWhenUnsuccessfulResponse(response: AxiosResponse) {
    if (response.status < 200 || response.status >= 300) {
        throw new Error("Unsuccessful response");
    }
    return response
}