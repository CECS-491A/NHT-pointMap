import axios from "axios";
import { api_url, sso_login_url } from "@/const.js"

function checkSession(){
    let urlString = api_url + '/api/session/'
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        }
    }
    axios.get(urlString, content).then((response) => {
        
    }).catch((err) => {
        if(err.response.status == 401){
            deleteSession()
        }
    })
};

function deleteSession(){
    let urlString = api_url + '/api/logout/session/'
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        }
    }
    axios.get(urlString, content).then((response) => {
        localStorage.removeItem('token');
        window.location.href = sso_login_url;
    }).catch((err) => {
        if(err.response.status == 401){
            localStorage.removeItem('token');
            window.location.href = sso_login_url;
        }
    })
}

export { checkSession, deleteSession };
