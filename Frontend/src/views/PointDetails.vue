<template>

    <div id = "pointdisplay" class="component" >
        <h1>Point Details</h1>
        <br />
        <v-form>
            <tt>Created At:</tt>
            <strong>
                {{createdAt}}
            </strong>
            <br /><br />
            <tt>Point Name:</tt>
    
            <strong>
                {{name}}
            </strong>

            <br /><br />
            <tt>Point Description:</tt>
            <input type ="text"  v-bind:placeholder= "description" >

            <br />
            <br />
            <tt>Longitude:</tt>
            <strong>
                {{long}}
            </strong>
            <br />
            <br />
            <tt>Latitude:</tt>
            <strong>
                {{lat}}
            </strong>
            <br />
            <br />
            <tt>Last Modified At:</tt>
            <strong>
                {{lastModifiedAt}}
            </strong>
            <br />
            <br />
            <tt>Point Id:</tt>
            <strong>
                {{id}}
            </strong>
            <br />
           
        </v-form>

        <v-btn color="success" v-on:click="pointEditor">Edit Point</v-btn>
    </div>


</template>

<script>

import {getPoint} from '../services/pointServices'
import { checkSession } from '../services/authorizationService';
import { LogWebpageUsage } from '@/services/loggingServices';

export default {
  name: 'PointDetails',
  data: function() {
    return {
      id: null,
      url: null,
      createdAt: null,
      lastModifiedAt: null,
      name: null,
      description: null,
      long: null,
      lat: null,
      logging: {
        webpage: '',
        webpageDurationStart: 0,
      }
    }
  },
  created() {
    this.logging.webpage = this.$options.name;
    this.logging.webpageDurationStart = Date.now();
  },
  destroyed() {
    const webpageDurationEnd = Date.now();
    LogWebpageUsage(this.logging.webpageDurationStart, webpageDurationEnd, this.logging.webpage);
  },
  methods: {
     getPointDetails: function()
     { 

         this.id =  this.$route.query.pointId
         this.rawData =getPoint(this.id, (arr)=>
         {
                if(arr!=null){
                    
                    this.createdAt = new Date(parseInt(arr[0].CreatedAt.substr(6)));
                    this.lastModifiedAt = new Date(parseInt(arr[0].UpdatedAt.substr(6)));
                    this.name = arr[0].Name;
                    this.description = arr[0].Description;
                    this.long = arr[0].Longitude;
                    this.lat = arr[0].Latitude;
                    
                }
         })
     },
     pointEditor: function() {
       this.$router.push({ path: 'pointeditor', query: { pointId: this.id } });
    }
  },
   beforeMount: function() {
      checkSession()
      this.getPointDetails()
  },
}
</script>

<style>
#pointdisplay{
     margin-bottom: 20px;
      width: 100%;
      height: 100px;
      left: 2px;
      background:whitesmoke;
      font-size: large;
      position: relative;
}
</style>

