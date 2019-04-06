<template>
  <v-toolbar dark color="primary">
    <v-menu  
        bottom 
        offset-y
        transition="slide-x-transition">
        <template v-slot:activator="{ on }">
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
        transition="slide-x-transition">
        <template v-slot:activator="{ on }">
            <v-btn v-on="on" fab dark color="teal">
                <v-avatar size=40 dark>
                    <span class="white--text headline">A</span>
                </v-avatar>
            </v-btn>
        </template>
        <v-list>
            <v-list-tile
                v-for="(item, i) in this.UserMenuItems"
                :key="i"
                @click=""
              >
                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
            </v-list-tile>
        </v-list>
    </v-menu>
  </v-toolbar>
</template>

<script>
export default {
  name: 'Navbar',
  data: () => ({
      ApplicationMenuItems: [
        { title: 'Admin Dashboard', link: "/admindashboard" }
      ],
      UserMenuItems: [
          {
              title: 'Logout'
          }
      ]
  })
}
</script>

<style>
.v-btn { 
    height: 40px;
    width: 40px
}
</style>