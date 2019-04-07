<template>
  <div>
    <div>
      <label>
        <gmap-autocomplete
          @place_changed="setPlace">
        </gmap-autocomplete>
        <button @click="addMarker">Search</button>
      </label>
      <br/>
    </div>
    <br>
    <gmap-map id="map"
      :center="center"
      :zoom="12"
      style="width:100%;  height: 600px;"
    >
      <gmap-marker
        :key="index"
        v-for="(m, index) in markers"
        :position="m.position"
        @click="center=m.position"
      ></gmap-marker>
    </gmap-map>
  </div>
</template>


<script>
import {getPoints} from '../services/pointServices'

export default {
  name: "MapView",
  data() {
    return {
      // default to Montreal to keep it simple
      // change this to whatever makes sense
      center: { lat: 45.508, lng: -73.587 },
      markers: [],
      places: [],
      currentPlace: null
    };
  },

  mounted() {
    this.geolocate();
    this.getToken();
  },

  methods: {
    // receives a place object via the autocomplete component
    setPlace(place) {
      this.currentPlace = place;
    },
    getToken(){
        localStorage.setItem('token', this.$route.query.token);
    },
    addMarker() {
      if (this.currentPlace) {
        const marker = {
          lat: this.currentPlace.geometry.location.lat(),
          lng: this.currentPlace.geometry.location.lng()
        };
        // this.markers.push({ position: marker });
        // this.places.push(this.currentPlace);
        this.center = marker;
        this.currentPlace = null;
      }
    },
    getPoints(){
        let points = pointService.getPoints(token, 10,11,-11,-10)
    },
    geolocate: function() {
      navigator.geolocation.getCurrentPosition(position => {
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
      });
    }
  }
};
</script>

<style>
    #map{
        margin-bottom: 20px;
    }
</style>
