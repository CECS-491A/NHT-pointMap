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

const UpdateUser = (user) =>
{
    return axios.put(`${api_url}/user/update`, user)
        .then(response =>
        {
            return response;
        })
}

const DeleteUser = (userId) =>
{
    return axios.delete(`${api_url}/user/delete/${userId}`)
        .then(response =>
        {
            return response;
        })
        .catch(Error =>
        {
            Error.response.status
        })
}

const GetUser = (token) =>
{
    return axios.get(`${api_url}/user/${token}`)
        .then(response =>
        {
            return response;
        })
}

export
{
    GetUsers,
    UpdateUser,
    DeleteUser,
    GetUser
}