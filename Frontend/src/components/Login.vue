<template>
    <h1>Welcome to login</h1>
</template>

<script>
import axios from 'axios'

    export default {
        name: 'Login',
        components: {},
        data() {
            return {

            }
        },
        mounted () {
            axios
                .post('http://localhost:58896/api/user/login')
                .then(response => (console.log(response)))
        }
    }
</script>