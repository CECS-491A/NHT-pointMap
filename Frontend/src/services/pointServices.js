import axios from "axios";

function getPoints(minLng, maxLng, minLat, maxLat, callback){
    let urlString = 'https://api.pointmap.net/api/points/'
    let content = {
        'headers':{
            'minLng': minLng,
            'maxLng': maxLng,
            'minLat': minLat,
            'maxLat': maxLat,
            'token': localStorage.getItem('token') || 'something'
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
    })
};

export{
    getPoints
}