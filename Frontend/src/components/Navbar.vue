<template>
  <v-toolbar dark color="primary">
    <v-menu  
        bottom 
        offset-y
        transition="slide-x-transition"
        v-if="stored.state.isLogin">
        <template v-if="true" v-slot:activator="{ on }">
            <v-btn
                dark
                icon
                v-on="on"
              >
            <v-icon>apps</v-icon>
            </v-btn>
        </template> 
        <v-list>
            <v-list-tile
                v-for="(item, i) in this.ApplicationMenuItems"
                :key="i"
                :to="{path: `${item.link}`}"
              >
                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
            </v-list-tile>
        </v-list>
    </v-menu>
    <v-toolbar-title class="white--text">PointMap</v-toolbar-title>
    <v-spacer></v-spacer>

    <v-menu
        bottom 
        offset-y
        transition="slide-x-transition"
        v-if="stored.state.isLogin">
        <template v-slot:activator="{on}">
            <v-btn v-on="on" fab dark color="teal">
                <v-avatar size=40 dark>
                    <span class="white--text headline">{{stored.state.email[0]}}</span>
                </v-avatar>
            </v-btn>
        </template>
        <v-list>
            <v-list-tile
                v-for="(item, i) in this.UserMenuItems"
                :key="i"
                v-on:click="item.action"
                :to="{path: `${item.link}`}"
              >
                <button>{{item.title}}</button>
            </v-list-tile>
        </v-list>
    </v-menu>
  </v-toolbar>
</template>

<script>
import {deleteSession} from '../services/authorizationService';
import { store, getUser } from '@/services/accountServices';


export default {
  name: 'Navbar',
  data: () => ({
      ApplicationMenuItems: [
        {title: 'Map View', link: '/mapview'},
      ],
      UserMenuItems: [
          { title: 'Account', link: '/account'},
          { title: 'Logout', action: deleteSession, link: '/' }
      ],
      user: {},
      stored: store
  }),
  updated() {
    getUser()
      .then(response => {
        switch(response.status){
          case 200:
            this.user = response.data;
            break;
        }
      })
  },
  mounted() {
    store.isUserLogin()
      if (store.state.isLogin === true) {
          store.getEmail();
          this.stored = store;
          this.$forceUpdate();
      }
  },
  methods:{
    logout(){
      deleteSession()
    }
  }
}
</script>

<style scoped>
.v-btn { 
    height: 40px;
    width: 40px
}
</style>