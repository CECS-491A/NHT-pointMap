import axios from "axios";
import { api_url } from "@/const.js";

const GetUsers = () =>
{
    return axios.get(`${api_url}/users`)
        .then(response =>
        {
            console.log(response);
            return response.data;
        })
}

export
{
    GetUsers
}