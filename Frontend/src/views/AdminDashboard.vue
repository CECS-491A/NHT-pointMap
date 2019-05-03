<template>
  <v-container 
    id="grid"
    fluid
    >
    <v-layout id="layout">
      <v-flex xs1.5>
        <v-navigation-drawer
          class="blue lighten-3"
          dark
          permanent
          width="210x"
        >
          <v-list>
            <v-list-tile
              id="navagationItem"
              v-for="item in DashboardMenuItems"
              :key="item.title"
              @click="SelectedComponent(item.title)"
            >
              <v-list-tile-action>
                <v-icon>{{ item.icon }}</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list>
        </v-navigation-drawer>
      </v-flex>
      <v-flex xs11>
        <UserManagement 
          v-if="this.$data.selectedItem === 'User Management'" 
          id="UserManagement"/>
        <AppPublish v-if="this.$data.selectedItem === 'App Publish'"/>
      </v-flex>
    </v-layout>

  </v-container>
</template>

<script>
import axios from 'axios'
import UserManagement from '@/components/UserManagement.vue'
import AppPublish from '@/components/AppPublish'
import {checkSession} from '../services/authorizationService'

    export default {
        name: 'AdminDashboard',
        components: {
          UserManagement,
          AppPublish
        },
        mounted(){
          checkSession();
        },
        data: () => ({
          selectedItem: 'User Management',
          DashboardMenuItems: [
              {title: 'User Management', icon: 'account_box'},
              {title: 'Analytics', icon: 'timeline'},
              {title: 'App Publish', icon: 'build'}
          ]
        }),
        methods: {
          SelectedComponent (component){
            this.$data.selectedItem = component;
          }
        },
    }
</script>

<style>
#grid {
  padding: 0px;
  height: 100%;
}

#layout {
  height: 100%;
}
#UserManagement {
  height: 100%
}
</style>