<template>
  <v-app id="inspire">
    <v-layout row justify center>
     
          <v-dialog
              v-model = "dialog"
              max-width = "500">
            <v-card>
              <v-card-title class = "headline"> Point Details</v-card-title>
            
                <v-card-text>
                    Created At:
                    <strong>
                        {{createdAt}}
                    </strong>
                    <br />
                      Point Name:
            
                    <strong>
                        {{name}}
                    </strong>

                    <br />
                    Point Description:
                    <strong>
                    {{description}}
                    </strong>

                    <br />
                   
                      Longitude:
                    <strong>
                        {{long}}
                    </strong>
                    <br />
                    
                    Latitude:
                    <strong>
                        {{lat}}
                    </strong>
                    <br />
                    
                    Last Modified At:
                    <strong>
                        {{lastModifiedAt}}
                    </strong>
                    <br />
                    
                      Point Id:
                    <strong>
                        {{id}}
                    </strong>
                    <br />
                  
                </v-card-text>
            <v-layout justify-left>
            <v-btn color="success" v-on:click="pointEditor">Edit Point</v-btn>
            </v-layout>
            <v-layout  justify-right>
            <v-btn color="error" v-on:click="deleteThePoint"
                @click="dialog=false">Delete Point</v-btn>
            </v-layout>
          </v-card>
        </v-dialog>
    </v-layout>
  </v-app>

</template>

<script>

import {getPoint} from '../services/pointServices'
import {deletePoint} from '../services/pointServices';
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
      },
      dialog:true
    }
  },
  watch: {
    dialog (val)
    {
      !val && this.$router.push('/mapview');  
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

     
    },
    deleteThePoint: function(){
        deletePoint(this.$route.query.pointId);
        this.$router.push('/mapview');

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

