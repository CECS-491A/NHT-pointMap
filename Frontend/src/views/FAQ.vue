<template>
    <v-container fill-height>
        <v-layout>
            <v-flex id="FAQ">
                <h1 id="faqHeader" class="display-1">Frequently Asked Questions (FAQ)</h1>
                <v-divider class='my-3'></v-divider>
                    <b class="subheading font-weight-black">What is Pointmap?</b>
                    <p class="answerText">Pointmap is a GeoSpatial mapping program capable of plotting and easily managing points of interest for an organization.</p>

                    <br/>
                    <b class="subheading font-weight-black">How do I edit points?</b>
                    <p class="answerText">Existing points can be edited by clicking on the appropriate marker that loads in the map. This will load a page
                       which displays the details of the clicked point. From this page, editing a point or deleting a point can be selected.</p>
                    
                    <br>
                    <b class="subheading font-weight-black">How do I register?</b>
                    <p class="answerText">Registration is completed through the single sign on application<a v-on:click="goToDocument(sso_registration)" class="link">here</a>.</p>

                    <br>
                    <b class="subheading font-weight-black">How do I log in?</b>
                    <p class="answerText">Login is completed after registration through the single sign on application <a v-on:click="goToDocument(sso_login)" class="link">here</a>. </p>

                    <br>
                    <b class="subheading font-weight-black">What is Pointmap's privacy policy?</b>
                    <p class="answerText">Pointmap's privacy policy can be viewed <router-link to='legal'>here</router-link>.</p>

                    <br>
                    <b class="subheading font-weight-black">Where can I find detailed information about how to use Pointmap?</b>
                    <p class="answerText">Additional documentation for Pointmap can be found<router-link to='documents'>here</router-link>.</p>

                    <br>
                    <b class="subheading font-weight-black">How can I delete my account?</b>
                    <p class="answerText">A user's account can be deleted by using the user avatar in the upper right hand corner and clicking on 'Account'. From there, the user can choose to delete their account from either Pointmap alone or Pointmap and the single sign on application.</p>
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
#faqHeader {
    text-align: center;
}

#FAQ{
    width: 100%;
    padding: 15px;
    margin-top: 20px;
    max-width: 900px;
    margin: 1px auto;
    align: center;
}

.answerText {
    padding-top: 15px;
}

.link{
    text-decoration: underline;
}
</style>
