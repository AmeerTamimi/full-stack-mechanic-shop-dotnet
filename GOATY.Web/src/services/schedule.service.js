import http from "./http.js";

export function getDailySchedule(params = {}, config = {}) {
  return http.get("/api/WorkOrders/schedule", { params, ...config });
}
