<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
  </div>
</template>

<script>

import axios from 'axios'

export default {
  name: 'HelloWorld',
  data () {
    return {
      msg: 'Welcome to Your Vue.js App'
    }
  },
  mounted () {
    axios
      .get('http://webapipointmap-dev.us-west-2.elasticbeanstalk.com/api/helloworld')
      // .get('https://api.coindesk.com/v1/bpi/currentprice.json')
      .then(response => (this.msg = response.data))
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h1, h2 {
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
