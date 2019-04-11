import axios from "axios";

function getPoints(minLng, maxLng, minLat, maxLat, callback){
    let urlString = 'https://api.pointmap.net/api/points/'
    let content = {
        'headers':{
            'minLng': minLng,
            'maxLng': maxLng,
            'minLat': minLat,
            'maxLat': maxLat,
            'token': 'A5D010EB0EB20E6EC842067715775D44E365EAE91D7284F8F0F8D69AF4C3B092'
        }
    }

    let arr = []
    axios.get(urlString, content).then((response) => {
        let data = response.data
        if(data){
            data.forEach(point => {
                let tempPoint = {
                    Latitude: point.Latitude, 
                    Longitude: point.Longitude,
                    Id: point.Id
                }
                arr.push(tempPoint);
            });
        }
        return callback(arr);
    }).catch((err) => {
        console.log(err);
        if(err.response.status == 401){
            localStorage.removeItem('token');
            window.location.href = 'https://kfc-sso.com/#/login';
        }
    })
    return null;
};
function grabProm(fetchurl)
{
    return axios.get(fetchurl);
};
function getPoint(pointId, callback){
    let arr =[]
    let urlString = 'https://api.pointmap.net/api/point/'+pointId
    grabProm(urlString).then((response) => {
        console.log(response.data)
        let data = response.data;

        arr.push(data);
        
        return callback(arr);
        
    }).catch((err) => {
        console.log(err);
        if (err.response.status == 401) {
            localStorage.removeItem('token');
            window.location.href = 'https://kfc-sso.com/#/login';
        }
    })
    return null;
};

export{
    getPoints,
    getPoint
}