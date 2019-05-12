<template>
    <v-container fill-height>
        <v-layout>
            <v-flex>
                <h1 id="Documents">Documents</h1>
                <v-divider class='my-3'></v-divider>
                <p>
                    <router-link to='policies'>Privacy Policy</router-link>
                    <br><br>

                    <a v-on:click="goToDocument(user_man)" class="link">Getting Started</a>
                    <br>
                </p>
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
#Documents{
    text-align: center;
}
.link{
    text-decoration: underline;
}
</style>
