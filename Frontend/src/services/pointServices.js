import axios from "axios";

function getPoints(minLng, maxLng, minLat, maxLat){
    let urlString = 'http://pointmap.me:8080/api/points'
    let content = {
        'headers':{
            'minLng': minLng,
            'maxLng': maxLng,
            'minLat': minLat,
            'maxLat': maxLat
        }
    }

    axios.get(urlString, content).then((response) => {
        let data = response.data
        print('\n', data)
    })
}