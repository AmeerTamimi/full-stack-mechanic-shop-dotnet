import http from "./http.js";

export function getDailySchedule(params = {}) {
  return http.get("/api/WorkOrders/schedule", { params });
}
