import { defineStore } from "pinia";
import { login as loginRequest, refreshToken as refreshTokenRequest } from "@/services/auth.service";
import { getJwtUser, hasAnyRole } from "@/utils/authToken";

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const EXPIRY_KEY = "expiry";

function readStoredAuth() {
  return {
    accessToken: localStorage.getItem(ACCESS_TOKEN_KEY),
    refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY),
    expiry: localStorage.getItem(EXPIRY_KEY),
  };
}

function buildState() {
  const storedAuth = readStoredAuth();

  return {
    ...storedAuth,
    user: getJwtUser(storedAuth.accessToken),
  };
}

export const useAuthStore = defineStore("auth", {
  state: () => buildState(),

  getters: {
    isAuthenticated: (state) => Boolean(state.accessToken),
    isTokenExpired: (state) => {
      if (!state.expiry) return true;

      return new Date(state.expiry).getTime() <= Date.now();
    },
    roles: (state) => state.user?.roles ?? [],
    primaryRole: (state) => state.user?.roles?.[0] ?? "",
    userId: (state) => state.user?.id ?? "",
    userEmail: (state) => state.user?.email ?? "",
    employeeId: (state) => state.user?.employeeId ?? "",
    hasEmployeeIdentity: (state) => Boolean(state.user?.employeeId),
    isManager: (state) => hasAnyRole(state.user?.roles ?? [], ["Manager"]),
    isTechnician: (state) => hasAnyRole(state.user?.roles ?? [], ["Technician"]),
    canAccessRoles: (state) => (allowedRoles = []) => hasAnyRole(state.user?.roles ?? [], allowedRoles),
  },

  actions: {
    setSession(data) {
      const accessToken = data.accessToken ?? data.AccessToken;
      const refreshToken = data.refreshToken ?? data.RefreshToken;
      const expiry = data.expiry ?? data.Expiry;

      this.accessToken = accessToken;
      this.refreshToken = refreshToken;
      this.expiry = expiry;
      this.user = getJwtUser(accessToken);

      localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
      localStorage.setItem(EXPIRY_KEY, expiry);
    },

    async login(email, password) {
      const { data } = await loginRequest({ email, password });
      this.setSession(data);
    },

    async refreshAccessToken() {
      if (!this.refreshToken) {
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
      this.user = null;

      localStorage.removeItem(ACCESS_TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
      localStorage.removeItem(EXPIRY_KEY);
    },

    restoreSession() {
      const stored = readStoredAuth();

      this.accessToken = stored.accessToken;
      this.refreshToken = stored.refreshToken;
      this.expiry = stored.expiry;
      this.user = getJwtUser(stored.accessToken);
    },
  },
});
