import axios from "axios";

function checkSession(){
    let urlString = 'https://api.pointmap.net/api/session/'
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        }
    }
    let arr = []
    axios.get(urlString, content).then((response) => {
        
    }).catch((err) => {
        if(err.response.status == 401){
            localStorage.removeItem('token');
            window.location.href = 'https://kfc-sso.com/#/login';
        }
    })
};

export{
    checkSession
}