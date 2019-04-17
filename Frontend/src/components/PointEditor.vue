<template>
  <div class="container">
    <h1>PointEditor</h1>

    <br />
    <v-form>
      Point Information:<br />
      <v-text-field
        name="name"
        id="name"
        v-model="name"
        type="name"
        label="Name" /><br />
      <v-text-field
        name="description"
        id="description"
        type="description"
        v-model="description"
        label="Description" /><br />
      <v-text-field
        name="longitude"
        id="longitude"
        v-model="longitude"
        label="Longitude" /><br />
      <v-text-field
        name="latitude"
        id="latitude"
        v-model="latitude"
        label="Latitude" /><br />

      <v-alert
        :value="error"
        type="error"
        transition="scale-transition"
      >
        {{error}}
      </v-alert>

      <br />

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
      id: '',
      name: '',
      description: '',
      latitude: '',
      longitude: '',
      createdAt: '',
      updatedAt: ''
    }
  },
  mounted(){
    checkSession();
  },
  methods: {
    getPointData: function() {
        this.id =  this.$route.query.pointId

        if(id !== '') {
            this.creatingPoint = false;
            this.rawData = getPoint(this.id, (arr)=> {
                if(arr!=null) {
                    this.createdAt = new Date(parseInt(arr[0].CreatedAt.substr(6)));
                    this.lastModifiedAt = new Date(parseInt(arr[0].UpdatedAt.substr(6)));
                    this.name = arr[0].Name;
                    this.description = arr[0].Description;
                    this.long = arr[0].Longitude;
                    this.lat = arr[0].Latitude;
                }
            })
        } else {
            this.creatingPoint = true;
        }
    },
    submit: function() {
      this.error = "";
      if (this.name == "") {
        this.error = "Point name is required.";
      } else if (this.description == "") {
        this.error = "Point description is required";
      } else if(this.latitude < 90 || this.latitude > 90 || 
                this.longitude < 180 | this.latitude > 180) {
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
            name: this.name,
            description: this.description,
            latitude: this.latitude,
            longitude: this.longitude,
            updatedAt: this.updatedAt,
            createdAt: this.createdAt,
            id: this.Id
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
</style>