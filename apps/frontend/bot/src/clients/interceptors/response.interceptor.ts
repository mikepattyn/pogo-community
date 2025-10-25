import { AxiosResponse } from "axios"
import { EnforceErrorGetsThrownWhenUnsuccessfulResponse } from "./functions/EnforceErrorGetsThrownWhenUnsuccessfulResponse"

export const ResponseInterceptor = (response: AxiosResponse) => {
    EnforceErrorGetsThrownWhenUnsuccessfulResponse(response)
}