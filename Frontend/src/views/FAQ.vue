<template>
    <v-container fill-height>
        <v-layout>
            <v-flex>
                <h1 id="FAQ">Facts and Questions</h1>
                <v-divider class='my-3'></v-divider>
                <p>
                    <b>What is Pointmap?</b>
                    <br>
                    Pointmap is a GeoSpatial mapping program capable of plotting and easily managing points of interest for an organization.
                    <br><br>

                    <b>How do I edit points?</b>
                    <br>
                    Existing points can be edited by clicking on the appropriate marker that loads in the map. This will load a page
                    which displays the details of the clicked point. From this page, editing a point or deleting a point can be selected.
                    <br><br>

                    <b>How do I register?</b>
                    <br>
                    Registration is completed through the single sign on application 
                    <a v-on:click="goToDocument(sso_registration)" class="link">here</a>.
                    <br><br>

                    <b>How do I log in?</b>
                    <br>
                    Login is completed after registration through the single sign on application 
                    <a v-on:click="goToDocument(sso_login)" class="link">here</a>.
                    <br><br>

                    <b>What is Pointmap's privacy policy?</b>
                    <br>
                    Pointmap's privacy policy can be viewed 
                    <router-link to='legal'>here</router-link>.
                    <br><br>

                    <b>Where can I find detailed information about how to use Pointmap?</b>
                    <br>
                    Pointmap's user manual can be viewed 
                    <a v-on:click="goToDocument(user_man)" class="link">here</a>.
                    <br><br>

                    <b>How can I delete my account?</b>
                    <br>
                    A user's account can be deleted by using the user avatar in the upper right hand corner and clicking on 'Account'. From there, the user can choose to delete their account from either Pointmap alone or Pointmap and the single sign on application.
                </p>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>

import { user_man_url, sso_login_url, sso_registration_url } from "@/const.js"

export default {
    name: 'FAQ',
    data: () => {
        return {
            user_man: user_man_url,
            sso_login: sso_login_url,
            sso_registration: sso_registration_url
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
#FAQ{
    text-align: center;
}
.link{
    text-decoration: underline;
}
</style>
