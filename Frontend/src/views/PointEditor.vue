<template>
  <div id="flexcontainer">
    <div id="pointeditor">
      <h1>Point Editor</h1>
      <v-form>
        <v-text-field
          name="name"
          id="name"
          v-model="point.name"
          type="name"
          label="Name" /> <br />
        <v-text-field
          name="description"
          id="description"
          type="description"
          v-model="point.description"
          label="Description" /><br />
        <v-text-field
          name="longitude"
          id="longitude"
          v-model.number="point.longitude"
          label="Longitude" 
          @change="updateMarkerPosition"/><br />
        <v-text-field
          name="latitude"
          id="latitude"
          v-model.number="point.latitude"
          label="Latitude" 
          @change="updateMarkerPosition"/><br />
        <br />
        <v-alert
          :value="error"
          type="error"
          transition="scale-transition"
        >
          {{error}}
        </v-alert>
        <v-btn color="success" v-on:click="submit">Save Point</v-btn>

      </v-form>
      <v-dialog
        v-model="loading"
        hide-overlay
        persistent
        width="300"
      >
        <v-card
          color="primary"
          dark
        >
          <v-card-text>
            Loading
            <v-progress-linear
              indeterminate
              color="white"
              class="mb-0"
            ></v-progress-linear>
          </v-card-text>
        </v-card>
      </v-dialog>
      <br />
      <div id="instruction">
        <h2>Drag the marker on the map to the desired location and 
          update the latitude/longitude of the marker. </h2>
      </div>
    </div>
    <div>
      <div id="map"></div> 
    </div>
  </div>
</template>

<script>

import {checkSession} from '../services/authorizationService'
import {updatePoint, createPoint, getPoint} from '../services/pointServices'

export default {
  name: 'PointEditor',
  data: () => {
    return {
      responseError: null,
      error: "",
      creatingPoint: false,
      loading: false,
      map: null,
      infoWindow: null,
      center: { 
        lat: 45.508, 
        lng: -73.587 
      },
      point: {
        id: '',
        name: '',
        description: '',
        latitude: '',
        longitude: '',
        createdAt: null,
        updatedAt: null
      },
      zoom: 18,
      marker: null
    }
  },
  mounted() {
    checkSession();
    this.getPointData();
    this.setupMap();
  },
  methods: {
    setupMap: function() {
      this.map = new google.maps.Map(document.getElementById('map'), {
        center: this.center,
        zoom: this.zoom,
      });

      let promise = new Promise((resolve, reject) => {
        if(this.creatingPoint) {
          navigator.geolocation.getCurrentPosition(position => {
            this.center = {
              lat: position.coords.latitude,
              lng: position.coords.longitude
            }
            resolve();
          });
        }
        else { resolve(); }
      })

      promise.then(() => {
        this.map.setCenter(this.center);
      }).then(() => {
        this.placeMarker();
      })
    },
    placeMarker: function() {
      let promise = new Promise((resolve, reject) => {
        this.marker = new google.maps.Marker({
          position: this.map.center,
          map: this.map,
          draggable: true
        });
        resolve();
      });
      
      promise.then(() => {
        this.marker.addListener('dragend', function(marker) {
          this.map.panTo(marker.latLng);
          this.point = {
            id: this.point.id,
            name: this.point.name,
            description: this.point.description,
            latitude: marker.latLng.lat(),
            longitude: marker.latLng.lng(),
            createdAt: this.point.CreatedAt,
            updatedAt: this.point.updateAt
          };
        }.bind(this));
      });
    },
    updateMarkerPosition: function() {
      this.center = {
        lat: this.point.latitude,
        lng: this.point.longitude
      };
      this.map.setCenter(this.center);
      this.marker.setPosition(this.map.center);
      this.map.panTo(this.map.center);
    },
    getPointData: function() {
      var pointId =  this.$route.query.pointId;

      if(pointId !== undefined) {
        this.creatingPoint = false;
        this.rawData = getPoint(pointId, (arr)=> {
          if(arr!=null) {
            this.point.createdAt = new Date(parseInt(arr.CreatedAt.substr(6)));
            this.point.updatedAt = new Date(parseInt(arr.UpdatedAt.substr(6)));
            this.point.name = arr.Name;
            this.point.description = arr.Description;
            this.point.longitude = arr.Longitude;
            this.point.latitude = arr.Latitude;
            this.center = {
              lat: this.point.latitude,
              lng: this.point.longitude
            };
          }
        })
        this.point.id = pointId;
      } else {
        this.creatingPoint = true;
        this.point = {
          id: this.point.id,
          name: this.point.name,
          description: this.point.description,
          latitude: this.center.lat,
          longitude: this.center.lng,
          createdAt: this.point.CreatedAt,
          updatedAt: this.point.updateAt
        };
      }
    },
    submit: function() {   
      this.error = "";

      if (this.point.name == "") {
        this.error = "Point name is required.";
      } else if (this.point.description == "") {
        this.error = "Point description is required.";
      } else if (this.point.latitude == "") {
        this.error = "Point latitude is required.";
      } else if(this.point.longitude == "") {
        this.error = "Point longitude is required.";
      }else if(parseInt(this.point.latitude) < -90 || parseInt(this.point.latitude) > 90 || 
               parseInt(this.point.longitude) < -180 || parseInt(this.point.longitude) > 180) {
        this.error = "Latitude/Longitude value(s) out of range."
      }

      if (this.error) return;

      this.loading = true;
      var func = null;
      if(this.creatingPoint) {
          func = createPoint;
      } else {
          func = updatePoint;
      }

      func({
        name: this.point.name,
        description: this.point.description,
        latitude: this.point.latitude,
        longitude: this.point.longitude,
        updatedAt: this.point.updatedAt,
        createdAt: this.point.createdAt,
        id: this.point.id
      }).then(() => {
          this.$router.push('mapview');
      }).catch(err => {
          switch(err.response.status) {
          case 401: 
              this.error = err.response.data;
              break;
          case 412:
              this.error = err.response.data;
              break;
          case 500:
              this.error = err.response.data;
          }
      }).finally(() => {
          this.loading = false;
      })
    }
  }
}
</script>

<style scoped>
  #map{
      margin-bottom: 20px;
      width: 650px;
      height: 650px;
      margin: 0 auto;
      background: gray;
  }
  #flexcontainer{
    display: flex;
    flex-flow: wrap;
    flex-direction: row;
    align-content: stretch;
    flex: 2;
    justify-content: space-between;
  }
  #pointeditor{
    width: 550px;
    margin: 20px;
  }
  #instruction{
    text-align: center;
  }
</style>