<template>
  <v-layout id="Account" xs>
  <div id="Account" v-if="user.id">
    <h1 class="display-1">Account Settings</h1>
    <v-divider class="my-3"></v-divider>
    <v-flex>
      <v-card>
        <v-card-title primary-title>
          <div>
            <h3 class="headline mb-0">{{this.user.username}}</h3>
          </div>
        </v-card-title>
      </v-card>
      <br/>
       <v-card>
        <v-card-title primary-title>
          <div>
            <h2 class="headline">Information</h2>
            <v-divider class="my-2"></v-divider>
            <br/>
            <h4 class="subheading">User ID: {{this.user.id}}</h4>
            <v-checkbox v-model="this.user.isAdmin" height="1" readonly label="Administrator" value="Admin"></v-checkbox>
            <v-checkbox v-model="this.user.disabled" height="1" readonly label="Disabled" value="Disabled"></v-checkbox>
          </div>
        </v-card-title>
      </v-card>
      <div id="Delete">
        <h2 class="headline">Account Deletion</h2>
        <v-btn id="deleteButton" color="error" @click="deleteFromPointmapAction">Delete From Pointmap</v-btn>
        <v-btn id="deleteButton"  wrap color="error" @click="deleteFromPointmapAndSSO">Delete From Pointmap and KFC SSO</v-btn>
      </div>
    </v-flex> 
  </div>
  <div v-if="loading">
    <Loading :dialog="loading" :text="loadingText"/>
  </div>
  <div v-if="popup">
    <PopupDialog :dialog="popup" :text="popupMessage" :redirect="doRedirect" :redirectUrl="redirectUrl"/>
  </div>
  </v-layout>
</template>
<script>

import { deleteAccountFromSSO, getUser, deleteAccountfromPointmap} from '@/services/accountServices'
import { httpResponseCodes } from '@/services/services.const.js'
import { LogWebpageUsage } from '@/services/loggingServices'
import Loading from '@/components/dialogs/Loading.vue'
import PopupDialog from '@/components/dialogs/PopupDialog.vue'

export default {
  name: 'Account',
  components: {
    Loading,
    PopupDialog,
  },
  data(){
      return{
        popup: false,
        popupMessage: '',
        doRedirect: false,
        redirectUrl: 'https://kfc-sso.com',
        token: "",
        error: "",
        message: "",
        user: {},
        loading: false,
        logging: {
          webpage: '',
          webpageDurationStart: 0,
        }
      }
  },
  created() {
    // Browser logger, listener if used switches/closes tab
    document.addEventListener('beforeunload', this.userBrowserTabSession)
    this.logging.webpage = this.$options.name;
    this.logging.webpageDurationStart = Date.now();
    window.addEventListener('beforeunload', this.userBrowserTabSession)
    this.getUserAccount();
  },
  destroyed() {
    const webpageDurationEnd = Date.now();
    LogWebpageUsage(this.logging.webpageDurationStart, webpageDurationEnd, this.logging.webpage);
  },
  methods: {
    getUserAccount () {
      this.loading = true;
      this.loadingText = 'Retrieving user data...';
      getUser()
      .then(response => {
        this.loading = false;
        switch(response.status){
          case httpResponseCodes.OK: // status OK
            this.user = response.data;
            break;
        }
      })
      .catch( err => {
        this.loading = false;
        switch(err.response.status){
          case httpResponseCodes.Unauthorized: // status Unauthorized
            this.doRedirect = true;
            this.popupMessage = 'Session has expired.';
            this.popup = true;
            localStorage.removeItem('token');
            break;
        }
      })
    },
    redirectToHome () {
      this.$router.push( "/home" )
    },
    deleteFromPointmapAndSSO () {
        this.loadingText = 'Deleteting from SSO and Pointmap...';
        this.loading = true;
        deleteAccountFromSSO()
            .then(response => {
              switch(response.status){
                case httpResponseCodes.OK:
                  this.doRedirect = true;
                  this.popupMessage = response.data;
                  this.popup = true;
                  localStorage.removeItem('token');
              }
            })
            .catch(e => { this.error = "Failed to delete user, try again" })
            .finally(() => { this.loading = false; })
    },
    deleteFromPointmapAction () {
      this.loadingText = 'Deleting from Pointmap...';
      this.loading = true;
      deleteAccountfromPointmap()
        .then(response => {
          this.loading = false;
          switch(response.status){
            case httpResponseCodes.OK:
              this.doRedirect = true;
              this.popupMessage = 'User has been deleted.';
              this.popup = true;
              localStorage.removeItem('token');
          }
        })
        .catch(err => {
          this.loading = false;
          switch(err.response.status){
            default:
              this.doRedirect = false;
              this.popupMessage = 'Failed to delete user, try again.';
              this.popup = true;
          }
        })
    },
    userBrowserTabSession(){
      const webpageDurationEnd = Date.now();
      LogWebpageUsage(this.logging.webpageDurationStart, webpageDurationEnd, this.logging.webpage);
    }
  }
}
</script>

<style>
#Account{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;	  
  align: center;
}

#Delete {
  padding-top: 15px;
  align: center;
}

#deleteButton {
  margin: 0px;
  margin-top: 10px;
}
</style>