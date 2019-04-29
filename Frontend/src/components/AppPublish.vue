<template>
    <div class="publish-wrapper">
        <h1>Update Pointmap - SSO</h1>
        <br />
        <v-form>
        <v-text-field
            name="key"
            id="key"
            v-model="key"
            type="key"
            label="API Key" 
            v-if="!validation"
            :loading="retrievingKey"
            :readonly="true"
            >
            </v-text-field>     
              <v-btn 
                id="generatekey" 
                color="success" 
                small
                v-on:click="generateKey">Generate Key</v-btn>       
            <br />
            <br/>
        <v-textarea
            name="description"
            id="description"
            type="description"
            v-model="description"
            label="Description"
            auto-grow
            v-if="!validation"
            rows="2"
            /><br />
        <v-text-field
            name="logoUrl"
            id="logoUrl"
            type="logoUrl"
            v-model="logoUrl"
            label="Logo Url" 
            v-if="!validation"
            /><br />
        <v-switch 
            id="underMaintenance"
            v-model="underMaintenance"
            :value=underMaintenance
            label="Application Under Maintenance"
            v-if="!validation">
        </v-switch>
        <v-alert
            :value="error"
            id="error"
            type="error"
            transition="scale-transition"
        >
            {{error}}
        </v-alert>

        <div v-if="validation" id="publishMessage">
            <h3>Successful Publish!</h3>
            <p>{{ validation }}</p>
        </div>
        <br />
        <v-btn id="btnPublish" color="success" v-if="!validation" v-on:click="publish">Publish</v-btn>
        </v-form>
    </div>
</template>

<script>
import axios from 'axios';
import { generateApiKey, updateApplicationInformation } from '@/services/kfcSSOServices';

export default {
  data () {
    return {
      validation: null,
      retrievingKey: false,
      key: '',
      title: 'Pointmap',
      adminEmail: 'admin@pointmap.com',
      description: 'Hello World, from Pointmap',
      logoUrl: 'https://media-hearth.cursecdn.com/avatars/thumbnails/245/497/55/55/635739598851922201.png',
      underMaintenance: false,
      error: '',
      loading: true,
      loadingText: ''
    }
  },
  methods: {
    publish: function () {
      this.error = "";
      if (this.key.length == 0 || this.title.length == 0 || this.description.length == 0 || this.logoUrl.length == 0) {
        this.error = "Fields Cannot Be Left Blank.";
      }
      if (this.error) return;
      updateApplicationInformation(this.key, this.title, this.description, this.logoUrl, this.underMaintenance)
        .then(response => {
          this.validation = response.data.message // Retrieve validation message from response
        })
        .catch(err => {
          this.error = err.response.data
        })
        .finally(() => {
          this.loading = false;
        })
    },
    generateKey: function() {
      this.key = '';
      this.retrievingKey = true;
      generateApiKey(this.title, this.adminEmail)
        .then(response => {
          this.key = response.data.Key;
        })
        .catch(err => {
          this.key = err.response.message;
        })
        .finally( () => {
          this.retrievingKey = false;
        }
        )
      }
  }
}
</script>

<style lang="css">
.publish-wrapper {
    width: 70%;
    margin-top: 20px;
    margin: 1px auto;
}
</style>