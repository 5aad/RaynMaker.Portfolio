import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import Positions from '@/views/Positions/Open/Positions.vue'
import Performance from '@/views/Performance/Performance.vue'
import Cashflow from '@/views/Cashflow/Cashflow.vue'
import ClosedPositions from '@/views/Positions/Closed/ClosedPositions.vue'
import PositionDetails from '@/views/Positions/Details/PositionDetails.vue'
import App from './App'
import CoreuiVue from '@coreui/vue'
const app = createApp(App)

const routes = [

  { path: '/Positions', alias: '/', component: Positions },
  { path: '/Cashflow', component: Cashflow },
  { path: '/Performance', component: Performance },
  { path: '/ClosedPositions', component: ClosedPositions },
  { path: '/PositionDetails', name: 'PositionDetails', component: PositionDetails }
]
const router = createRouter({
  history: createWebHistory(),
  routes,
  linkActiveClass: 'active',
  linkExactActiveClass: 'active'
})
app.use(CoreuiVue)
app.use(router)
app.mount('#app')
