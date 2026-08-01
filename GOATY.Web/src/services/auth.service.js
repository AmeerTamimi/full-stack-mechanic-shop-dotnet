import http from './http';

export function login(payload){
    return http.post("/api/identity/token/generate", payload, {
        skipAuthRefresh: true,
    });
}

export function refreshToken(token) {
    return http.post("/api/identity/token/refresh-token", { token }, {
        skipAuthRefresh: true,
        skipLoader: true,
    });
}
