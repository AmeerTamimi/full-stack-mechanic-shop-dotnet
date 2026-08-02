import http from "./http.js";

export function getDashboard(params = {}) {
  return http.get("/api/Dashboard", { params });
}
