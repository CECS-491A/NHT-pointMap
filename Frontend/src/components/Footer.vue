<template>
  <v-footer dark primary height="auto">
    <v-layout justify-center row wrap primary>
      <v-btn v-for="link in links" :key="link" flat round @click="route(link)">{{ link }}</v-btn>
      <v-flex primary lighten-2 text-xs-center white--text xs12 sm21 md12 lg12 xl12>
        &copy;{{ new Date().getFullYear() }} —
        <strong>PointMap</strong>
      </v-flex>
    </v-layout>
  </v-footer>
</template>

<script>
import { dev_docs_url, user_man_url } from "@/const.js"

export default {
  name: "Navbar",
  data: () => ({
    links: [
      "Home",
      "Privacy Policy",
      "FAQ",
      "Developer Documents",
      "User Manual"
    ]
  }),
  methods: {
    route: function(link) {
        switch(link) {
            case "Home": this.$router.push('/mapview'); break;
            case "Privacy Policy": this.$router.push('/legal'); break;
            case "FAQ": this.$router.push('/faq'); break;
            case "User Manual": this.goToDocuments(user_man_url); break;
            case "Developer Documents": this.goToDocuments(dev_docs_url); break;
        }
    },
    goToDocuments: function(url) {
        var form = document.createElement("form");

        // Opens in new tab
        form.target = "_blank";

        form.method = 'GET';
        // Where the form is redirecting
        form.action = url;
        // Appends the child the the form can be submitted
        document.body.appendChild(form);
        // Submits the form
        form.submit();
        // Removes the childs so it cannot be accessed after it is submitted
        document.body.removeChild(form);
    }
  }
};
</script>