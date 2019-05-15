import axios from "axios";
import { api_url } from "@/const.js";
import { deleteSession } from "./authorizationService.js"

function getPoints(minLng, maxLng, minLat, maxLat, callback) {
  let urlString = `${api_url}/api/points/`;
  let content = {
    headers: {
      minLng: minLng,
      maxLng: maxLng,
      minLat: minLat,
      maxLat: maxLat,
      token: localStorage.getItem("token")
    }
  };

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
            deleteSession();
        }
      });
  return null;
}

function getPoint(pointId, callback) {
  let content = {
    headers: {
      token: localStorage.getItem("token")
    }
  }
  let arr = [];
  let urlString = `${api_url}/api/point/` + pointId;
  axios
    .get(urlString, content)
    .then(response => {
      let data = response.data;
      arr.push(data);
      return callback(arr);
    })
    .catch(err => {
      console.log(err);
      if (err.response.status == 401) {
        deleteSession();
      }
    });
  return null;
}

function updatePoint(point){
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        },
    }
    axios.put(`${api_url}/api/point`, point, content)
        .then((response) => {
            return response.data;
        
        }).catch((err) => {
            console.log(err);
            if (err.response.status == 401) {
                deleteSession();
            }
        })
    return null;
};

function createPoint(point){
    let content = {
        'headers':{
            'token': localStorage.getItem('token')
        },
    }
    axios.post(`${api_url}/api/point`, point, content)
        .then((response) => {
            return response.data;
            
        }).catch((err) => {
            console.log(err);
            if (err.response.status == 401) {
                deleteSession();
            }
        })
    return null;
};
function deletePoint(pointId) {
  let content = {
    'headers': {
      'token': localStorage.getItem('token')
    },
  }
  let urlString = `${api_url}/api/point/` + pointId;
  axios
    .delete(urlString)
    .then(response => {

      return response.data;
    })
    .catch(err => {
      console.log(err);
      if (err.response.status == 401) {
        deleteSession();
      }
    });
  return null;
};

export{
    getPoints,
    getPoint,
    updatePoint,
    createPoint,
    deletePoint
}
