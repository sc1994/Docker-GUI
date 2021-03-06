import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
  routes: [{
      path: '/',
      name: 'home',
      component: () => import('./views/home.vue')
    }, {
      path: '/log/:id/:name',
      name: 'log',
      component: () => import('./views/log.vue'),
      props: true
    }, {
      path: '/stats/:id/:name',
      name: 'stats',
      component: () => import('./views/stats.vue'),
      props: true
    }, {
      path: '/stats-new/:id/:name',
      name: 'statsNew',
      component: () => import('./views/stats-new.vue'),
      props: true
    }
    // {
    //   path: '/about',
    //   name: 'about',
    //   // route level code-splitting
    //   // this generates a separate chunk (about.[hash].js) for this route
    //   // which is lazy-loaded when the route is visited.
    //   component: () => import(/* webpackChunkName: "about" */ './views/About.vue')
    // }
  ]
})