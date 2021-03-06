import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '../components/HelloWorld'
import Login from '@/views/Login'
import AdminDashboard from '../views/AdminDashboard'
import MapView from '../views/MapView'
import PointDetails from '../views/PointDetails'
import PointEditor from '../views/PointEditor'
import Account from '@/views/Account'
import PrivacyPolicy from '@/views/PrivacyPolicy'
import FAQ from '@/views/FAQ'
import Documents from '@/views/Documents'


Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/admindashboard',
      name: 'AdminDashboard',
      component: AdminDashboard
    },
    {
      path: "/mapview",
      name: "MapView",
      component: MapView
    },
    {
      path: "/pointdetails",
      name: "PointDetails",
      component: PointDetails
    },
    {
      path: "/pointeditor",
      name: "PointEditor",
      component: PointEditor
    },
    {
      path: "/account",
      name: "Account",
      component: Account
    },
    {
      path: "*",
      component: MapView
    },
    {
      path: "/login",
      name: "Login",
      component: Login
    },
    {
      path: "/policies",
      name: "LegalAndPrivacy",
      component: PrivacyPolicy
    },
    {
      path: '/faq',
      name: 'FAQ',
      component: FAQ
    },
    {
      path: '/documents',
      name: 'Documents',
      component: Documents
    }
  ]
});
