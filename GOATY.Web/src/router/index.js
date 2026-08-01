import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";

const routes = [
    {
        path: "/login",
        name: "login",
        component: () => import("@/views/Login/LoginView.vue"),
        meta: {
            guestOnly: true,
        },
    },
    {
        path: "/",
        name: "home",
        component: () => import("@/views/Dashboard/DashboardView.vue"),
        meta: {
            requiresAuth: true,
        },
    },
    {
        path: "/parts",
        name: "parts",
        component: () => import("@/views/Parts/PartListView.vue"),
        meta: {requiresAuth: true},
    },
    {
        path: "/parts/create",
        name: "part-create",
        component: () => import("@/views/Parts/PartCreateView.vue"),
        meta: {requiresAuth: true},
    },
    {
        path: "/parts/:id/edit",
        name: "part-edit",
        component: () => import("@/views/Parts/PartEditView.vue"),
        meta: {requiresAuth: true},
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

router.beforeEach(async (to) => {
    const auth = useAuthStore();
    const ui = useUiStore();

    ui.resetLoader();
    auth.restoreSession();

    if (to.meta.requiresAuth && !auth.isAuthenticated) {
        return {
            name: "login",
            query: {
                redirect: to.fullPath,
            },
        };
    }

    if (to.meta.requiresAuth && auth.isTokenExpired) {
        const refreshedToken = await auth.refreshAccessToken();

        if (!refreshedToken) {
            return {
                name: "login",
                query: {
                    redirect: to.fullPath,
                },
            };
        }
    }

    if (to.meta.guestOnly && auth.isAuthenticated && !auth.isTokenExpired) {
        return {
            name: "home",
        };
    }

    return true;
});

export default router;
