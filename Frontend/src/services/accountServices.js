import axios from "axios";
import { api_url } from "@/const.js";

let config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

const DeleteAccountFromSSO = () => {
  config.headers.Token = localStorage.getItem("token");
  return axios.post(`${api_url}/api/user/deletefromsso`).then(response => {
    return response;
  });
};

export const deleteAccountfromPointmap = () => {
  console.log("deleting");
  config.headers.Token = localStorage.getItem("token");
  return axios.delete(`${api_url}/api/user/delete`, config);
};

export const getUser = () => {
  config.headers.Token = localStorage.getItem("token");
  return axios.get(`${api_url}/user`, config);
};

let token = localStorage.getItem("token");

export const store = {
  state: {
    isLogin: false,
    email: ""
  },
  isUserLogin() {
    if (token !== null) {
      this.state.isLogin = true;
    } else {
      this.state.isLogin = false;
    }
  },
  getEmail() {
    config.headers.Token = localStorage.getItem("token");
    axios.get(`${api_url}/user`, config).then(resp => {
      console.log(resp);
      this.state.email = resp.data.username;
    });
  }
};

export { DeleteAccountFromSSO };
