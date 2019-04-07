import axios from "axios";
import { api_url } from "@/const.js";

const config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

const GetUsers = () =>
{
    return axios.get(`${api_url}/users`,config)
        .then(response =>
        {
            console.log(response);
            return response.data;
        })
        
}

const UpdateUser = (user) =>
{
    return axios.put(`${api_url}/user/update`, user, config)
        .then(response =>
        {
            return response;
        })
}

const DeleteUser = (userId) =>
{
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
    return axios.get(`${api_url}/user`, config)
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