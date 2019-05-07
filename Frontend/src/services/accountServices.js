import axios from "axios";
import { api_url } from "@/const.js";

let config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

export const deleteAccountFromSSO = () => {
  config.headers.Token = localStorage.getItem("token");
  return axios.delete(`${api_url}/api/user/deletefromsso`, config);
};

export const deleteAccountfromPointmap = () => {
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
