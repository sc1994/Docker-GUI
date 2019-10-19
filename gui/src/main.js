import Vue from 'vue'
import './plugins/axios'
import App from './App.vue'
import store from './store'
import router from './router'
import './plugins/element.js'
import VCharts from 'v-charts'

Vue.config.productionTip = false
Vue.use(VCharts)

new Vue({
  store,
  router,
  render: h => h(App)
}).$mount('#app')