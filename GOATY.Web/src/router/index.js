import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";
import { operationDelayMs, waitForMinimumDuration } from "@/utils/operationDelay";

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
        component: () => import("@/layouts/DashboardLayout.vue"),
        meta: {
            requiresAuth: true,
        },
        children: [
            {
                path: "",
                name: "home",
                component: () => import("@/views/Dashboard/DashboardView.vue"),
                meta: { title: "Dashboard" },
            },
            {
                path: "parts",
                name: "parts",
                component: () => import("@/views/Parts/PartListView.vue"),
                meta: { title: "Parts" },
            },
            {
                path: "parts/create",
                name: "part-create",
                component: () => import("@/views/Parts/PartCreateView.vue"),
                meta: { title: "Create part" },
            },
            {
                path: "parts/:id/edit",
                name: "part-edit",
                component: () => import("@/views/Parts/PartEditView.vue"),
                meta: { title: "Edit part" },
            },
            {
                path: "repair-tasks",
                name: "repair-tasks",
                component: () => import("@/views/RepairTasks/RepairTaskListView.vue"),
                meta: { title: "Repair tasks" },
            },
            {
                path: "repair-tasks/create",
                name: "repair-task-create",
                component: () => import("@/views/RepairTasks/RepairTaskCreateView.vue"),
                meta: { title: "Create repair task" },
            },
            {
                path: "repair-tasks/:id/edit",
                name: "repair-task-edit",
                component: () => import("@/views/RepairTasks/RepairTaskEditView.vue"),
                meta: { title: "Edit repair task" },
            },
            {
                path: "employees",
                name: "employees",
                component: () => import("@/views/Employees/EmployeeListView.vue"),
                meta: { title: "Employees" },
            },
            {
                path: "employees/create",
                name: "employee-create",
                component: () => import("@/views/Employees/EmployeeCreateView.vue"),
                meta: { title: "Create employee" },
            },
            {
                path: "employees/:id/edit",
                name: "employee-edit",
                component: () => import("@/views/Employees/EmployeeEditView.vue"),
                meta: { title: "Edit employee" },
            },
            {
                path: "customers",
                name: "customers",
                component: () => import("@/views/Customers/CustomerListView.vue"),
                meta: { title: "Customers" },
            },
            {
                path: "customers/create",
                name: "customer-create",
                component: () => import("@/views/Customers/CustomerCreateView.vue"),
                meta: { title: "Create customer" },
            },
            {
                path: "customers/:id/edit",
                name: "customer-edit",
                component: () => import("@/views/Customers/CustomerEditView.vue"),
                meta: { title: "Edit customer" },
            },
        ],
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

async function showNavigationDelay(ui) {
    if (!operationDelayMs) return;

    const startedAt = Date.now();

    ui.changeShowLoader(true);

    try {
        await waitForMinimumDuration(startedAt);
    } finally {
        ui.changeShowLoader(false);
    }
}

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

    await showNavigationDelay(ui);

    return true;
});

export default router;
