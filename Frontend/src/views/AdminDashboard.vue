<template>
   <v-navigation-drawer
    class="blue lighten-3"
    dark
    permanent
    width="210x"
  >
    <v-list>
      <v-list-tile
        v-for="item in DashboardMenuItems"
        :key="item.title"
        @click=""
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
</template>

<script>
import axios from 'axios'

    export default {
        name: 'Dashboard',
        components: {},
        data: () => ({
            DashboardMenuItems: [
                {title: 'User Management', icon: 'account_box'},
                {title: 'Analytics', icon: 'timeline'}
            ]
        }),
        mounted () {
        }
    }
</script>