<template>
  <v-layout id="Account" xs>
  <div id="Account" v-if="user.id">
    <v-flex>
      <v-card>
        <v-card-title primary-title>
          <div>
            <h3 class="headline mb-0">{{this.user.username}}</h3>
            <h4>ID: {{this.user.id}}</h4>
          </div>
        </v-card-title>
      </v-card>
        <div id="Delete">
          <h2>Account Deletion</h2>
          <v-btn color="error" @click="deleteFromPointmapAction">Delete From Pointmap</v-btn>
          <v-btn wrap color="error" @click="deleteFromPointmapAndSSO">Delete From Pointmap and KFC SSO</v-btn>
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

import { DeleteAccountFromSSO, getUser, deleteAccountfromPointmap} from '@/services/accountServices'
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
      }
  },
  created() {
    this.getUserAccount();
  },
  methods: {
    getUserAccount () {
      this.loading = true;
      this.loadingText = 'Retrieving user data...';
      getUser()
      .then(response => {
        this.loading = false;
        switch(response.status){
          case 200:
            this.user = response.data;
            break;
        }
      })
      .catch( err => {
        this.loading = false;
        switch(err.response.status){
          case 401:
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
        this.loading = true;
        DeleteAccountFromSSO()
            .then(response => {
                this.doRedirect = true;
              this.popupMessage = 'User has been deleted.';
              this.popup = true;
              localStorage.removeItem('token');
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
            case 200:
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
    }
  }
}
</script>

<style>
#Account{
  width: 100%;
  margin-top: 20px;
  max-width: 700px;
  margin: 1px auto;
}

#Delete {
  padding-top: 15px;
  padding-left: 15px;
  align: center;
}
</style>