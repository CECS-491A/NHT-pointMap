import axios from "axios";
import { api_url } from "@/const.js";

let config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

const GetUsers = () =>
{
    config.headers.Token = localStorage.getItem("token");
    return axios.get(`${api_url}/users`,config)
        .then(response =>
        {
            return response.data;
        })
        
}

const UpdateUser = (user) =>
{
    config.headers.Token = localStorage.getItem("token");
    return axios.put(`${api_url}/user/update`, user, config)
        .then(response =>
        {
            return response;
        })
}

const DeleteUser = (userId) =>
{
    config.headers.Token = localStorage.getItem("token");
    return axios.delete(`${api_url}/user/delete/${userId}`, config)
        .then(response =>
        {
            return response;
        })
        .catch(Error =>
        {
            Error.response.status
        })
}

const GetUser = () =>
{
    config.headers.Token = localStorage.getItem("token");
    return axios.get(`${api_url}/user`, config)
        .then(response =>
        {
            return response;
        })
}

export const createNewUser = () =>
{
    config.headers.Token = localStorage.getItem('token');
    
}

export
{
    GetUsers,
    UpdateUser,
    DeleteUser,
    GetUser
}