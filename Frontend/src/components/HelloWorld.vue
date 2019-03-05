<template>
  <div class='HelloWorld'>
  <h1 v-if="msg != null">{{msg}}</h1>
  <div id="loader" v-else> 
    <BeatLoader/>
  </div>
  </div>
</template>

<script>
import '@babel/polyfill'
import { BeatLoader } from '@saeris/vue-spinners'
import axios from 'axios'

export default {
  name: 'HelloWorld',
  components: {
    BeatLoader
  },
  data() {
    return {
      msg: null
    }
  },
  mounted () {
    axios
      .get('http://localhost:58896/api/helloworld')
      .then(response => (this.msg = response.data))
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
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

#loader {
  padding: 25px;
}
</style>
 