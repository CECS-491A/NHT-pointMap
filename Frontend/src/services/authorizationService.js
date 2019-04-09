import axios from "axios";

function checkSession(){
    let urlString = 'https://api.pointmap.net/api/session/'
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        }
    }
    axios.get(urlString, content).then((response) => {
        
    }).catch((err) => {
        if(err.response.status == 401){
            localStorage.removeItem('token');
            window.location.href = 'https://kfc-sso.com/#/login';
        }
    })
};

function deleteSession(){
    let urlString = 'https://api.pointmap.net/api/logout/session/'
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        }
    }
    axios.get(urlString, content).then((response) => {
        localStorage.removeItem('token');
        window.location.href = 'https://kfc-sso.com/#/login';
    }).catch((err) => {
        if(err.response.status == 401){
            localStorage.removeItem('token');
            window.location.href = 'https://kfc-sso.com/#/login';
        }
    })
};

export{
    checkSession,
    deleteSession
}