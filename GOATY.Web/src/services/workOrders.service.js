import http from "./http.js";

export function getWorkOrders(params = {}, config = {}) {
  return http.get("/api/WorkOrders", { params, ...config });
}

export function getWorkOrder(id) {
  return http.get(`/api/WorkOrders/${id}`);
}

export function addWorkOrder(payload) {
  return http.post("/api/WorkOrders", payload);
}

export function assignWorkOrderTechnician(id, employeeId) {
  return http.put(`/api/WorkOrders/${id}/technician`, { employeeId });
}

export function relocateWorkOrder(id, payload) {
  return http.put(`/api/WorkOrders/${id}/relocation`, payload);
}

export function updateWorkOrderVehicle(id, vehicleId) {
  return http.put(`/api/WorkOrders/${id}/vehicle`, { vehicleId });
}

export function updateWorkOrderState(id, state) {
  return http.put(`/api/WorkOrders/${id}/state`, { state });
}

export function updateWorkOrderRepairTasks(id, repairTaskIds) {
  return http.put(`/api/WorkOrders/${id}/repair-tasks`, {
    workOrderRepairTasks: repairTaskIds.map((repairTaskId) => ({ repairTaskId })),
  });
}

export function deleteWorkOrder(id) {
  return http.delete(`/api/WorkOrders/${id}`);
}
