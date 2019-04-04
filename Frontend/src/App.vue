<template>
  <div>
    <Navigation></Navigation>
    <div id="app">
      <router-view/>
    </div>
  </div>
</template>

<script>
import Navigation from './components/Nav'

export default {
  name: 'App',
  components: {
    'Navigation': Navigation
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 10px;
}

#logo{
  width: 30%;
  height: 30%
}
</style>
