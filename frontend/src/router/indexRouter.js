import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStores'
import ProfileView from '@/views/ProfileViews.vue'
import ChangePasswordView from '@/views/ChangePasswordView.vue'

const routes = [
    {
        path: '/login',
        name: 'Login',
        component: () => import('@/views/loginViews.vue'),
        meta: { requireGuest: true}
    },
    {
        path: '/register',
        name: 'Register',
        component: () => import('@/views/registerViews.vue'),
        meta: { requireGuest: true}
    },
    {
        path: '/profile',
        name: 'Profile',
        component: ProfileView,
        meta: { requiresAuth: true }
    },
    {
        path: '/change-password',
        name: 'ChangePassword',
        component: ChangePasswordView,
        meta: { requiresAuth: true }
    },
    {
        path: '/verify',
        name: 'Verification',
        component: () => import('@/views/VerificationView.vue')
    },
    {
        path: '/',
        name: 'Home',
        component: () => import('@/views/homeViews.vue'),
        meta: { requiresAuth: true}
    },

    {
        path: '/:pathmatch(.*)*',
        redirect: '/login'
    }
]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore()
    const isAuthenticated = authStore.isAuthenticated

    if (to.meta.requiresAuth && !isAuthenticated) {
        next('/login')
    } else if (to.meta.requireGuest && isAuthenticated) {
        next('/')
    } else {
        next()
    }
})

export default router