<template>
    <v-container fill-height>
        <v-layout>
            <v-flex id="documentPage">
                <h1 id="header" class="display-1">Documents</h1>
                <v-divider class='my-3'></v-divider>
                
                <h2 class="headline">Frequently Asked Questions - FAQ</h2>
                <p class="body-1">Visit our FAQ page to help answer your questions about Pointmap.</p>
                <router-link to='faq'>Frequently Asked Questions (FAQ)</router-link>
                <br>
                
                <h2 class="headline">Privacy Policy</h2>
                <p class="body-1">Visit our Privacy Policy page to learn more about Pointmap's Privacy Policy.</p>
                <router-link to='policies'>Privacy Policy</router-link>
                <br>
                
                <h2 class="headline">Developer Docs</h2>
                <p class="body-1">Visit our User Manual to learn how to learn more and get started with Pointmap's API.</p>
                <a v-on:click="goToDocument(user_man)" class="link">Developer Docs</a>
                <br>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>

import { user_man_url, dev_docs_url } from "@/const.js"

export default {
    name: 'Documents',
    data: () => {
        return {
            user_man: user_man_url,
            dev_docs_url: dev_docs_url
        }
    },
    methods: {
        goToDocument: function(url) {
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
}
</script>

<style scoped>
#documentPage {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 900px;
  margin: 1px auto;
  align: center;
}

#header{
    text-align: center;
}
.link{
    text-decoration: underline;
}
.body-1 {
  padding-top: 15px;
  margin-bottom: 5px
}

.headline {
  margin-top: 50px;
}

</style>
