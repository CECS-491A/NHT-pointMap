import axios from "axios";

function getPoints(minLng, maxLng, minLat, maxLat){
    let urlString = 'https://api.pointmap.net/api/points'
    let content = {
        'headers':{
            'minLng': minLng,
            'maxLng': maxLng,
            'minLat': minLat,
            'maxLat': maxLat,
            'token': localStorage.getItem('token')
        }
    }

    axios.get(urlString, content).then((response) => {
        let data = response.data
        print('\n', data)
    })
}

export{
    getPoints
}