<template>
  <div>
    <h1>Welcome to login</h1>
    <h2>{{ this.token }}</h2>
     <div v-if="loading">
      <Loading :dialog="loading" :text="loadingText"/>
    </div>
    <div v-if="!validSession">
      <PopupDialog :dialog="!validSession" :text="popupMessage" redirect=true redirectUrl="https://kfc-sso.com"/>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import Loading from '@/components/dialogs/Loading'
import PopupDialog from '@/components/dialogs/PopupDialog'
import { GetUser } from '@/services/userManagementServices'

export default {
  name: 'Login',
  components: {
    Loading,
    PopupDialog
  },
  data: () => ({
    token: '',
    loading: false,
    loadingText: '',
    validSession: true,
    popupMessage: ''
  }),
  created() {
    this.token = this.$route.query.token;
    this.CheckUser(this.token);
    // localStorage.setItem('token', this.$route.query.token)
  },
  methods: {
    CheckUser(token) {
      this.loading = true;
      this.loadingText = 'Logging In...';
      GetUser(token)
        .then( response => {
          switch(response.status){
            case 200:
              var user = response.data;
              localStorage.setItem('token', this.token);
              this.loading = false;
              this.loadingText = '';
              if (user.isAdmin){
                this.$router.push('/admindashboard');
              }
            default:
          }
        })
        .catch( err => {
          this.loading = false;
          this.loadingText = '';
          switch(err.response.status){
            case 404:
              this.loading = false;
              this.popupMessage = 'The session has expired...';
              this.validSession = false;
              break;
            default:
          }
        })
    }
  }
}
</script>