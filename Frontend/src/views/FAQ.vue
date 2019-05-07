<template>
    <v-container fill-height>
        <v-layout>
            <v-flex>
                <h1 id="FAQ">Facts and Questions</h1>
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
                    <a href='http://kfc-sso.com'>here</a>.
                    <br><br>

                    <b>How do I log in?</b>
                    <br>
                    Login is completed after registration through the single sign on application 
                    <a href='http://kfc-sso.com'>here</a>.
                    <br><br>

                    <b>What is Pointmap's privacy policy?</b>
                    <br>
                    Pointmap's privacy policy can be viewed 
                    <a href='/legal'>here</a>.
                    <br><br>

                    <b>Where can I find detailed information about how to use Pointmap?</b>
                    <br>
                    Pointmap's user manual can be viewed 
                    <a href='/usermanual'>here</a>.
                </p>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'FAQ'
}
</script>

<style scoped>
    #FAQ{
        text-align: center;
    }
</style>
