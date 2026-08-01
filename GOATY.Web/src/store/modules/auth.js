import {defineStore} from 'pinia';
import {login as loginRequest, refreshToken as refreshTokenRequest} from '@/services/auth.service';

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const EXPIRY_KEY = "expiry";

function readStoredAuth(){
    return{
        accessToken: localStorage.getItem(ACCESS_TOKEN_KEY),
        refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY),
        expiry: localStorage.getItem(EXPIRY_KEY),
    }
}

export const useAuthStore = defineStore('auth',{
    state : () => ({
       ...readStoredAuth(),
    }),

    getters: {
        isAuthenticated: (state) => Boolean(state.accessToken),
        isTokenExpired: (state) => {
            if(!state.expiry) return true;

            return new Date(state.expiry).getTime() <= Date.now();
        },
    },

    actions: {
        setSession(data) {
            const accessToken = data.accessToken ?? data.AccessToken;
            const refreshToken = data.refreshToken ?? data.RefreshToken;
            const expiry = data.expiry ?? data.Expiry;

            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
            this.expiry = expiry;

            localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
            localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
            localStorage.setItem(EXPIRY_KEY, expiry);
        },

        async login(email, password) {
            const { data } = await loginRequest({ email, password });
            this.setSession(data);
        },
        async refreshAccessToken() {
            if(!this.refreshToken){
                this.logout();
                return null;
            }

            const { data } = await refreshTokenRequest(this.refreshToken);
            this.setSession(data);

            return this.accessToken;
        },
        logout() {
            this.accessToken = null;
            this.refreshToken = null;
            this.expiry = null;

            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            localStorage.removeItem("expiry");
        },
        restoreSession() {
            const stored = readStoredAuth();

            this.accessToken = stored.accessToken;
            this.refreshToken = stored.refreshToken;
            this.expiry = stored.expiry;
        },
    },
});
