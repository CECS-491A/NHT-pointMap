<template>
  <div>
    <h1>Welcome to login</h1>
    <h2>{{ this.token }}</h2>
     <div v-if="loading">
      <Loading :dialog="loading" :text="loadingText"/>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import Loading from '@/components/dialogs/Loading'
import { GetUser } from '@/services/userManagementServices'

export default {
  name: 'Login',
  components: {
    Loading
  },
  data: () => ({
    token: '',
    loading: false,
    loadingText: ''
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
              break;
            default:
          }
        })
        .catch( err => {
          this.loading = false;
          this.loadingText = '';
        })
    }
  }
  }
</script>