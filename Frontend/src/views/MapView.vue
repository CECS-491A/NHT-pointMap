<template>
  <!-- <div>
    <div>
      <label style="margin-top: 20px;margin-left:20px;border: 1px solid #ccc;">
        <gmap-autocomplete
          @place_changed="setPlace">
        </gmap-autocomplete>
      </label>
      <button style="margin-top: 20px;background-color:#e7e7e7;padding:0px 25px"  @click="addMarker">Search</button>
      <br/>
    </div>
    <br>
    <gmap-map id="map"
      :center="center"
      :zoom="currentZoom"
      @click="setZoom"
      style="width:100%;  height: 600px;"
    >
      <gmap-marker
        :key="index"
        v-for="(m, index) in markers"
        :position="m.position"
        @click="center=m.position"
      ></gmap-marker>
    </gmap-map>
  </div> -->
  <div>
    <!-- <div>
      <label style="margin-top: 20px;margin-left:20px;border: 1px solid #ccc;">
        <gmap-autocomplete
          @place_changed="setPlace">
        </gmap-autocomplete>
      </label>
      <button style="margin-top: 20px;background-color:#e7e7e7;padding:0px 25px"  @click="goTo">Search</button>
      <br/>
    </div>
    <br> -->
    <div v-on:click="requestPoints" id="map"></div>
  </div>
</template>

<script>
import {getPoints} from '../services/pointServices'
import {gmapApi} from 'vue2-google-maps'

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
      marker: null
    }
  },
  mounted: function () {
    this.map = new google.maps.Map(document.getElementById('map'), {
      center: this.center,
      zoom: this.zoom
    });
    this.infoWindow = new google.maps.InfoWindow;
    this.geolocate();
    this.getToken();
  },
  methods:{
    geolocate: function() {
      navigator.geolocation.getCurrentPosition(position => {
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        this.map.setCenter(this.center);
      });
    },
    setPlace(place) {
    this.currentPlace = place;
    },
    getToken(){
      localStorage.setItem('token', this.$route.query.token);
    },
    goTo() {
      if (this.currentPlace) {
        const marker = {
          lat: this.currentPlace.geometry.location.lat(),
          lng: this.currentPlace.geometry.location.lng()
        };
        this.center = marker;
        this.map.setCenter(this.center);
        this.currentPlace = null;
        this.getPoints();
      }
    },
    requestPoints(){
      this.zoom = this.map.getZoom();
      this.center = {
        lat: this.map.getCenter().lat(),
        lng: this.map.getCenter().lng()
      }
      this.metersPerPixel = 156543.03392 * Math.cos(this.center.lat * Math.PI / 180) / Math.pow(2, this.zoom)
      this.width = this.map.getDiv().offsetWidth
      this.height = this.map.getDiv().offsetHeight
      this.widthMeters = this.width * this.metersPerPixel //Longtitude
      this.heightMeters = this.height * this.metersPerPixel //Latitude
      this.mapBorder = {
        minLat: this.center.lat - (this.heightMeters/111111), 
        maxLat: this.center.lat + (this.heightMeters/111111), 
        minLng: this.center.lng - (this.widthMeters/111111), 
        maxLng: this.center.lng + (this.widthMeters/111111)
      }

      this.points = getPoints(this.mapBorder.minLng, this.mapBorder.maxLng, this.mapBorder.minLat, this.mapBorder.maxLat, (arr) => {
        this.markers = []
        this.points = arr
        this.points.forEach(point => {
          this.marker = new google.maps.Marker({
            position: 
            {
              lat: point.Latitude, 
              lng: point.Longitude
            },
            map: this.map,
            title: point.Id
          });
          this.markers.push(this.marker)
        })
      });
    }
  }  
};
</script>

<style>
  #map{
      margin-bottom: 20px;
      width: 100%;
      height: 550px;
      margin: 0 auto;
      background: gray;
  }
</style>
