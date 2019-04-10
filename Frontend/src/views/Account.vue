<template>
  <div id="delete">
    <div id="DeleteAccount">
     <h1>Account Deletion</h1>
    </div>
      <v-alert
      :value="message"
      dismissible
      type="success"
    >
      {{message}}
    </v-alert>

    <v-alert
      :value="error"
      dismissible
      type="error"
      transition="scale-transition"
    >
    {{error}}
    </v-alert>
    <br />
    <br />
    <div class="">
        <br/>
        <v-btn color="error" v-on:click="runDelete">Delete My Account</v-btn>
    </div>

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
          Deleting
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

import { DeleteAccountFromSSO } from '@/services/accountServices'

export default {
  name: 'Account',
  data(){
      return{
          token: "",
          error: "",
          message: "",
          loading: false
      }
  },
  methods: {
    redirectToHome: function () {
      this.$router.push( "/home" )
    },
    runDelete: function () {
        this.loading = true;
        DeleteAccountFromSSO()
            .then(response => {
                this.message = response.data;
                localStorage.removeItem('token'),
                window.location.href = 'https://kfc-sso.com/#/login';
            })
            .catch(e => { this.error = "Failed to delete user, try again" })
            .finally(() => { this.loading = false; })
    }
  }
}
</script>

<style>
#delete{
  width: 70%;
  margin: 1px auto;
}
</style>