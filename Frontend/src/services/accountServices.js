import axios from "axios";
import { api_url } from "@/const.js";

let config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

const DeleteAccountFromSSO = () =>
{
    config.headers.Token = localStorage.getItem("token");
    return axios.post(`${api_url}/api/user/deletefromsso`)
        .then(response =>
        {
            return response
        })
}

export
{
    DeleteAccountFromSSO
}