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
                
                <h2 class="headline">User Manual</h2>
                <p class="body-1">Select a User Manual to learn more about how to operate Pointmap's features.</p>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/UserManuals/Admin Dashboard - Analytics - User Manual.pdf" download>Admin Dashboard - Analytics</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/UserManuals/Admin Dash - UM - User Manual Doc.pdf" download>Admin Dashboard - User Management</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/UserManuals/Map View - User Manual.pdf" download>Map View</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/UserManuals/Point Editor_ User Manual.pdf" download>Point Editor</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/UserManuals/SSO_ Launch User Manual.pdf" download>SSO Launch</a>
                </div>
                <br>
                
                <h2 class="headline">Developer Documents</h2>
                <p class="body-1">Visit our Developer Documents to learn detailed information about Pointmap.</p>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/Admin Dash - Analytics - Developer Doc.pdf" download>Admin Dashboard - Analytics</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/Admin Dash - UM - Developer Doc.pdf" download>Admin Dashboard - User Management</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/Logging Developer Document.pdf" download>Logging</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/Map View_ Developer Docs.pdf" download>Map View</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/Point Editor_ Developer Documents.pdf" download>Point Editor</a>
                </div>
                <div  class="document-li-container">
                    <img src="/static/images/pdf_icon.png" class="image-pdf-icon"><a class="document-link" href="/static/DeveloperDocs/SSO_ Launch Developer Implementation Documentation.pdf" download>SSO Launch</a>
                </div>
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
            dev_docs: dev_docs_url
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

.document-li-container{
  display: flex;
  align-items: center;
}

.document-link{
    vertical-align: middle;
    padding-left: 8px;
    margin-bottom: 0;
}

.image-pdf-icon{
    width: 15px;
    height: 15px;
}
</style>
