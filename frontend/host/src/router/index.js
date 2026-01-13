import { createRouter, createWebHistory } from 'vue-router'
import HomePage from '@/pages/HomePage.vue'
import axios from 'axios'
import { listenRemoteRoutes } from './remoteRoutes.js';

const routes = [
  {
    path: '/',
    name: 'home',
    meta: { public: true }, // This route is public
    component: HomePage,
  },
  {
    path: '/register/:campaignName',
    name: 'Register',
    meta: { public: true }, // This route is public
    component: () => import("registration/App"),
  },
  {
    path: '/campaigns/:campaignName',
    name: 'RecruitmentCampaign',
    component: () => import("campaigns/RecruitingCampaignView"),
  },
  {
    path: '/campaigns',
    name: 'Campaigns',
    component: () => import("campaigns/CampaignsView"),
  },
  {
    path: '/schedule_interviews/:campaignName',
    name: 'InterviewScheduling',
    component: () => import("scheduling/App"),
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach(async (to, from, next) => {
  const isAuthenticated = await axios.get('/userinfo', { withCredentials: true })
    .then(response => {
      return response.status === 200
    }
    ).catch(() => false)

  console.log("Navigating to:", to.fullPath, "Authenticated:", isAuthenticated);
  if (to.meta.public) {
    console.log("Public route, no auth needed");
    next()
  } else if (!isAuthenticated){
    console.log("Not authenticated, redirecting to login");
    next(false) // Cancel the navigation
    window.location.href = '/login' // Redirect to login if not authenticated
  } else {
    console.log("Authenticated, proceeding to route");
    next()
  }
})

listenRemoteRoutes(router);

export default router
