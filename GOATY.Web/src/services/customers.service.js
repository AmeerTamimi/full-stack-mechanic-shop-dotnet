import http from "./http.js";

export function getCustomers(params = {}) {
  return http.get("/api/Customers", { params });
}

export function getCustomer(id) {
  return http.get(`/api/Customers/${id}`);
}

export function addCustomer(payload) {
  return http.post("/api/Customers", payload);
}

export function updateCustomer(id, payload) {
  return http.put(`/api/Customers/${id}`, payload);
}

export function deleteCustomer(id) {
  return http.delete(`/api/Customers/${id}`);
}
