import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";
import { operationDelayMs, waitForMinimumDuration } from "@/utils/operationDelay";

const MANAGER_ROLES = ["Manager"];
const WORK_ORDER_ROLES = ["Manager", "Technician"];

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
                meta: { title: "Dashboard", roles: MANAGER_ROLES },
            },
            {
                path: "parts",
                name: "parts",
                component: () => import("@/views/Parts/PartListView.vue"),
                meta: { title: "Parts", roles: MANAGER_ROLES },
            },
            {
                path: "schedule",
                name: "schedule",
                component: () => import("@/views/Schedule/ScheduleView.vue"),
                meta: { title: "Schedule", roles: WORK_ORDER_ROLES },
            },
            {
                path: "work-orders",
                name: "work-orders",
                component: () => import("@/views/WorkOrders/WorkOrderListView.vue"),
                meta: { title: "Work orders", roles: WORK_ORDER_ROLES },
            },
            {
                path: "work-orders/create",
                name: "work-order-create",
                component: () => import("@/views/WorkOrders/WorkOrderCreateView.vue"),
                meta: { title: "Create work order", roles: MANAGER_ROLES },
            },
            {
                path: "work-orders/:id",
                name: "work-order-details",
                component: () => import("@/views/WorkOrders/WorkOrderDetailsView.vue"),
                meta: { title: "Work order details", roles: WORK_ORDER_ROLES },
            },
            {
                path: "parts/create",
                name: "part-create",
                component: () => import("@/views/Parts/PartCreateView.vue"),
                meta: { title: "Create part", roles: MANAGER_ROLES },
            },
            {
                path: "parts/:id/edit",
                name: "part-edit",
                component: () => import("@/views/Parts/PartEditView.vue"),
                meta: { title: "Edit part", roles: MANAGER_ROLES },
            },
            {
                path: "repair-tasks",
                name: "repair-tasks",
                component: () => import("@/views/RepairTasks/RepairTaskListView.vue"),
                meta: { title: "Repair tasks", roles: MANAGER_ROLES },
            },
            {
                path: "repair-tasks/create",
                name: "repair-task-create",
                component: () => import("@/views/RepairTasks/RepairTaskCreateView.vue"),
                meta: { title: "Create repair task", roles: MANAGER_ROLES },
            },
            {
                path: "repair-tasks/:id/edit",
                name: "repair-task-edit",
                component: () => import("@/views/RepairTasks/RepairTaskEditView.vue"),
                meta: { title: "Edit repair task", roles: MANAGER_ROLES },
            },
            {
                path: "employees",
                name: "employees",
                component: () => import("@/views/Employees/EmployeeListView.vue"),
                meta: { title: "Employees", roles: MANAGER_ROLES },
            },
            {
                path: "employees/create",
                name: "employee-create",
                component: () => import("@/views/Employees/EmployeeCreateView.vue"),
                meta: { title: "Create employee", roles: MANAGER_ROLES },
            },
            {
                path: "employees/:id/edit",
                name: "employee-edit",
                component: () => import("@/views/Employees/EmployeeEditView.vue"),
                meta: { title: "Edit employee", roles: MANAGER_ROLES },
            },
            {
                path: "customers",
                name: "customers",
                component: () => import("@/views/Customers/CustomerListView.vue"),
                meta: { title: "Customers", roles: MANAGER_ROLES },
            },
            {
                path: "customers/create",
                name: "customer-create",
                component: () => import("@/views/Customers/CustomerCreateView.vue"),
                meta: { title: "Create customer", roles: MANAGER_ROLES },
            },
            {
                path: "customers/:id/edit",
                name: "customer-edit",
                component: () => import("@/views/Customers/CustomerEditView.vue"),
                meta: { title: "Edit customer", roles: MANAGER_ROLES },
            },
        ],
    },
    {
        path: "/forbidden",
        name: "forbidden",
        component: () => import("@/views/System/ForbiddenView.vue"),
        meta: {
            requiresAuth: true,
            title: "Forbidden",
        },
    },
    {
        path: "/:pathMatch(.*)*",
        name: "not-found",
        component: () => import("@/views/System/NotFoundView.vue"),
        meta: {
            title: "Not found",
        },
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

function getMatchedRoles(to) {
    return [
        ...new Set(
            to.matched.flatMap((record) => {
                const roles = record.meta.roles;

                if (!roles) {
                    return [];
                }

                return Array.isArray(roles) ? roles : [roles];
            })
        ),
    ];
}

async function refreshExpiredSession(auth, ui) {
    try {
        return await auth.refreshAccessToken();
    } catch {
        auth.logout();
        ui.showErrorToast("Your session expired. Please sign in again.", "Session expired");
        return null;
    }
}

router.beforeEach(async (to) => {
    const auth = useAuthStore();
    const ui = useUiStore();
    const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);
    const guestOnly = to.matched.some((record) => record.meta.guestOnly);
    const allowedRoles = getMatchedRoles(to);

    ui.resetLoader();
    auth.restoreSession();

    if (requiresAuth && !auth.isAuthenticated) {
        return {
            name: "login",
            query: {
                redirect: to.fullPath,
            },
        };
    }

    if (requiresAuth && auth.isTokenExpired) {
        const refreshedToken = await refreshExpiredSession(auth, ui);

        if (!refreshedToken) {
            return {
                name: "login",
                query: {
                    redirect: to.fullPath,
                },
            };
        }
    }

    if (requiresAuth && allowedRoles.length && !auth.canAccessRoles(allowedRoles)) {
        return {
            name: "forbidden",
            query: {
                from: to.fullPath,
            },
        };
    }

    if (guestOnly && auth.isAuthenticated && !auth.isTokenExpired) {
        return {
            name: "home",
        };
    }

    await showNavigationDelay(ui);

    return true;
});

export default router;
