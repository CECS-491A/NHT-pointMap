<template>
  <div>
    <div v-on:click="requestPoints" id="map"></div> 
  </div>
</template>

<script>
import {getPoints} from '../services/pointServices'
import {gmapApi} from 'vue2-google-maps'
import {checkSession} from '../services/authorizationService'
import MarkerClusterer from "@google/markerclusterer"

export default {
  name: "MapView",
  props: ['name'],
  data: function () {
    return {
      map: null,
      infoWindow: null,
      center: { lat: 45.508, lng: -73.587 },
      mapBorder: {minLat:0.1, maxLat:0.1, minLng:0.1, maxLng:0.1},
      zoom: 12,
      currentPlace: null,
      metersPerPixel: null,
      height: null,
      width: null,
      widthMeters: 1.0,
      heightMeters: 1.0,
      points: [],
      markers: [],
      marker: null,
      markerCluster: null,
      title: null
    }
  },
  mounted: function () {
    checkSession()
    this.map = new google.maps.Map(document.getElementById('map'), {
      center: this.center,
      zoom: this.zoom,
      zoomControlOptions: {
        position: google.maps.ControlPosition.LEFT_BOTTOM
      },
      streetViewControlOptions: {
        position: google.maps.ControlPosition.LEFT_BOTTOM
      },
    });
    var addPointDiv = document.createElement('div');
    
    this.setupPointButton(addPointDiv, this.map);

    this.map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(addPointDiv);
    this.infoWindow = new google.maps.InfoWindow;
    this.geolocate();
  },
  methods:{
    geolocate: function() {
      navigator.geolocation.getCurrentPosition(position => {
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        this.map.setCenter(this.center); //Needs timeout for the map to get a chance to center
      })
      var promise = new Promise((resolve, reject) => { //Sets a timeout for 1 second when called
        setTimeout(() => { 
          resolve()
        }, 1000);
      })
      promise.then(() => {
        this.requestPoints(); //Request points after timeout
      })
    },
    clearPoints(){
      for (var i = 0; i < this.markers.length; i++) {
          this.markers[i].setMap(null); //Clears all markers from map by changing the map they are set to, to null
      }
      this.markers = [] //Clears array list
    },

    requestPoints(){
      this.clearPoints() //Clears points so as to not add any duplicate markers on request
      this.zoom = this.map.getZoom();
      this.center = {
        lat: this.map.getCenter().lat(),
        lng: this.map.getCenter().lng()
      }
      this.metersPerPixel = 156543.03392 * Math.cos(this.center.lat * Math.PI / 180) / Math.pow(2, this.zoom) //Equation to retrieve pixels per meter
      this.width = this.map.getDiv().offsetWidth //Gets the pixels of width and height
      this.height = this.map.getDiv().offsetHeight 
      this.widthMeters = this.width * this.metersPerPixel //Longtitude in meters
      this.heightMeters = this.height * this.metersPerPixel //Latitude in meters
      this.mapBorder = {
        minLat: this.center.lat - (this.heightMeters/111111), //Gets minimum and maximum degrees to load points 
        maxLat: this.center.lat + (this.heightMeters/111111), 
        minLng: this.center.lng - (this.widthMeters/111111), 
        maxLng: this.center.lng + (this.widthMeters/111111)
      }

      //Request for points from backend, returns an array of Point objects
      this.points = getPoints(this.mapBorder.minLng, this.mapBorder.maxLng, this.mapBorder.minLat, this.mapBorder.maxLat, (arr) => {
        if(arr != null){ 
          this.points = arr
          var promise = new Promise((resolve, reject) => { //Promise of looping through points is created
            this.points.forEach(point => {
              this.marker = new google.maps.Marker({ //Creates a new point for every point returned
                position: 
                {
                  lat: point.Latitude,
                  lng: point.Longitude
                },
                map: this.map,
                title: point.Id
              });
              this.marker.addListener('click', function() { //Adds an event listener to each point to reroute to pointDetails page
                window.location.href = 'http://pointmap.net/#/pointdetails/?pointId=' + point.Id
              });
              this.markers.push(this.marker)
              resolve()
            })
          })
          promise.then(() => { //After promise is resolved creates cluster on existing points
            this.createCluster();
          })
        }
      });
    },
    createCluster(){
      this.markerCluster = new MarkerClusterer(this.map, this.markers, //Creates a cluster object which clusters all markers on the map
        {imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m'});
    },
    setupPointButton(addPointDiv){
      addPointDiv.style.borderRadius = "50%";
      addPointDiv.style.backgroundColor = "teal";
      addPointDiv.style.color = "#fff";
      addPointDiv.style.height = "56px";
      addPointDiv.style.width = "56px";
      addPointDiv.style.marginRight = "17px";
      addPointDiv.style.marginBottom = "2px";
      addPointDiv.style.boxShadow = '0 0 4px 2px rgba(0,0,0,.3)';
      addPointDiv.align = 'center';

      var addPointBtn = document.createElement('div');
      addPointBtn.style.fontFamily = 'Roboto,Arial,sans-serif';
      addPointBtn.style.fontSize = '30px';
      addPointBtn.textContent = '+';
      addPointBtn.style.paddingTop = '10px';
      addPointBtn.style.height = '56px';
      addPointBtn.style.width = '56px';
      addPointBtn.style.cursor = 'pointer';

      addPointBtn.addEventListener('click', function() {
        window.location.href = 'http://pointmap.net/#/pointeditor';
      });
      
      addPointDiv.appendChild(addPointBtn);
    }
  }  
};
</script>

<style>
  #map{
      margin-bottom: 20px;
      width: 100%;
      height: 650px;
      margin: 0 auto;
      background: gray;
  }

</style>
