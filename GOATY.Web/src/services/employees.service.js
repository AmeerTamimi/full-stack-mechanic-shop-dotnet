import http from "./http.js";

export function getEmployees(params = {}) {
  return http.get("/api/Employees", { params });
}

export function getEmployee(id) {
  return http.get(`/api/Employees/${id}`);
}

export function addEmployee(payload) {
  return http.post("/api/Employees", payload);
}

export function updateEmployee(id, payload) {
  return http.put(`/api/Employees/${id}`, payload);
}

export function deleteEmployee(id) {
  return http.delete(`/api/Employees/${id}`);
}
