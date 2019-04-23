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
        <v-alert
          :value="error"
          type="error"
          transition="scale-transition"
        >
          {{error}}
        </v-alert><br />
        <v-btn color="success" v-on:click="submit">Save Point</v-btn>

      </v-form>
      <div v-if="loading">
        <Loading :dialog="loading" :text="loadingText"/>
      </div>
      <br />
      <div id="instruction">
        <h2>Drag the marker on the map to the desired location. </h2>
      </div>
    </div>
    <div>
      <div id="map"></div> 
    </div>
  </div>
</template>

<script>

import Loading from '@/components/dialogs/Loading.vue'
import {checkSession} from '../services/authorizationService'
import {updatePoint, createPoint, getPoint} from '../services/pointServices'

export default {
  name: 'PointEditor',
  components: {
    Loading
  },
  data: () => {
    return {
      responseError: null,
      loadingText: "",
      error: "",
      creatingPoint: false,
      loading: false,
      map: null,
      infoWindow: null,
      center: { 
        lat: '', 
        lng: '' 
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
      //displays loading message if map is delayed loading
      this.loadingText = "Map loading";
      this.loading = true;

      //ensures that this.center is on the location of the user or point being updated before loading the map
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
        else { resolve(); } //if the point is already created, this.center has already been set
      })

      promise.then(() => {
        this.map = new google.maps.Map(document.getElementById('map'), {
          center: this.center,
          zoom: this.zoom,
        });
        //removes loading message
        this.loadingText = "";
        this.loading = false;
      }).then(() => {
        this.placeMarker(); //then places the marker at the center of the map
      })
    },
    placeMarker: function() {
      //ensures that the marker is placed and centered before a listener is added
      let promise = new Promise((resolve, reject) => {
        this.marker = new google.maps.Marker({
          position: this.map.center,
          map: this.map,
          draggable: true
        });
        //updates latitude/longitude form fields
        this.point.latitude = this.marker.position.lat();
        this.point.longitude = this.marker.position.lng();
        resolve();
      });
      
      promise.then(() => {
        //will automatically pan the map center to the dragged location
        this.marker.addListener('dragend', function(marker) {
          this.map.panTo(marker.latLng);
          //updates latitude/longitude form fields
          this.point.latitude = marker.latLng.lat();
          this.point.longitude = marker.latLng.lng();
        }.bind(this));
      });
    },
    //updates the postion of the marker if the form fields are edited
    updateMarkerPosition: function() {
      //ensures that latitude longitude fields cannot be edited to an empty string
      //  this prevents an error caused when updating the marker location
      if(this.point.latitude == "" || this.point.longitude == "") {
        this.point.latitude = this.center.lat;
        this.point.longitude = this.center.lng;

        //displays an error message
        this.error = "Latitude/longitude values cannot be empty.";
        var promise = new Promise((resolve, reject) => {
          setTimeout(() => { //error messages is displayed for 2 seconds
            resolve()
          }, 2000);
        })
        promise.then(() => {
          this.error = ""; // error message is removed
        });
      }
      else {
        this.center = {
          lat: this.point.latitude,
          lng: this.point.longitude
        };
        //resets the map center and marker position to be the longitude/latitude values in the form fields
        //  then pans the map to the set center position
        this.map.setCenter(this.center); 
        this.marker.setPosition(this.map.center);
        this.map.panTo(this.map.center);
      }
    },
    getPointData: function() {
      var pointId = this.$route.query.pointId;

      //enters here from point details
      if(pointId !== undefined) { //needs to be tested
        this.creatingPoint = false;
        getPoint(pointId, (point) => {
          if(arr!=null) {
            this.point = point; 
            this.center = {
              lat: this.point.latitude,
              lng: this.point.longitude
            };
          }
        })
        this.point.id = pointId;
      } else { //enters here from map view
        this.creatingPoint = true;
      }
    },
    submit: function() {   
      this.error = "";

      //ensures all required fields are filled in and valid
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

      var func = null;
      //Dynamically loads either the create or update functions based upon whether a point is 
      //  being created or updated. Also displays a specific message for each case.
      if(this.creatingPoint) {
          func = createPoint;
          this.loadingText = "Creating point."
      } else {
          func = updatePoint;
          this.loadingText = "Updating point."
      }
      this.loading = true;

      //calls the pointServices.[func(point)] to call the backend and perform the operation
      let promise = new Promise((resolve, reject) => {
        func(this.point);
        resolve();
      });

      promise.then(() => {
        //after operation, sends the user back to the mapview
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
    width: 590px;
    margin: 20px;
  }
  #instruction{
    text-align: center;
  }
</style>