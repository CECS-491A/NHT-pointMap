import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '../components/HelloWorld'
import Login from '../components/Login'
import Dashboard from '../components/Dashboard'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'HelloWorld',
      component: HelloWorld
    },
    {
        path: '/login',
        name: 'Login',
        component: Login
    },
    {
      path: '/Dashboard',
      name: 'Dashboard',
      component: Dashboard
    }
  ]
})
