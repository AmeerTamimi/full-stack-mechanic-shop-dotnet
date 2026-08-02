import http from "./http.js";

export function getRepairTasks(params = {}) {
  return http.get("/api/RepairTasks", { params });
}

export function getRepairTask(id) {
  return http.get(`/api/RepairTasks/${id}`);
}

export function addRepairTask(payload) {
  return http.post("/api/RepairTasks", payload);
}

export function updateRepairTask(id, payload) {
  return http.put(`/api/RepairTasks/${id}`, payload);
}

export function deleteRepairTask(id) {
  return http.delete(`/api/RepairTasks/${id}`);
}
