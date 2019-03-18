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
                .post('http://localhost:58896/api/user/login', 
                {'SSOUserId': '12093523',
                'Email': 'asdasd@gmail.com',
                'Timestamp': 23454252,
                'Signature': 'D078F2AFC7E59885F3B6D5196CE9DB716ED459467182A19E04B6261BBC8E36EE'})
                .then(response => (console.log(response)))
        }
    }
</script>