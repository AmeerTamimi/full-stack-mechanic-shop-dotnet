import axios from "axios";
import { useAuthStore } from "@/store/modules/auth.js";
import { useUiStore } from "@/store/modules/ui.js";
import { waitForMinimumDuration } from "@/utils/operationDelay.js";

const http = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
});

let refreshPromise = null;

function startGlobalLoader(config) {
    if (config.skipLoader) return;

    const ui = useUiStore();
    ui.changeShowLoader(true);
    config.usesGlobalLoader = true;
    config.loaderStartedAt = Date.now();
}

async function stopGlobalLoader(config) {
    if (!config?.usesGlobalLoader) return;

    await waitForMinimumDuration(config.loaderStartedAt);

    const ui = useUiStore();
    ui.changeShowLoader(false);
}

http.interceptors.request.use(async (config) => {
    const auth = useAuthStore();

    startGlobalLoader(config);

    try {
        if (!config.skipAuthRefresh && auth.accessToken && auth.isTokenExpired && auth.refreshToken) {
            refreshPromise = refreshPromise ?? auth.refreshAccessToken();
            await refreshPromise;
            refreshPromise = null;
        }

        if (auth.accessToken && !auth.isTokenExpired) {
            config.headers.Authorization = `Bearer ${auth.accessToken}`;
        }

        return config;
    } catch (error) {
        refreshPromise = null;
        await stopGlobalLoader(config);
        throw error;
    }
});

http.interceptors.response.use(
    async (response) => {
        await stopGlobalLoader(response.config);
        return response;
    },
    async (error) => {
        const auth = useAuthStore();
        const originalRequest = error.config;

        await stopGlobalLoader(originalRequest);

        if (error.response?.status !== 401 || originalRequest?._retry) {
            return Promise.reject(error);
        }

        originalRequest._retry = true;

        try {
            refreshPromise = refreshPromise ?? auth.refreshAccessToken();
            const newAccessToken = await refreshPromise;
            refreshPromise = null;

            if (!newAccessToken) {
                throw error;
            }

            originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

            return http(originalRequest);
        } catch (refreshError) {
            refreshPromise = null;
            auth.logout();

            if (window.location.pathname !== "/login") {
                window.location.href = "/login";
            }

            return Promise.reject(refreshError);
        }
    }
);

export default http;
