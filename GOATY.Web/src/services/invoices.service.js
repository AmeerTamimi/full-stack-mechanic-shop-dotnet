import http from "./http.js";

export function getInvoices(params = {}) {
  return http.get("/api/Invoices", { params });
}

export function getInvoice(id) {
  return http.get(`/api/Invoices/${id}`);
}

export function getInvoicePdf(id) {
  return http.get(`/api/Invoices/pdf/${id}`);
}

export function issueInvoice(workOrderId) {
  return http.post(`/api/Invoices/${workOrderId}`);
}

export function settleInvoice(id) {
  return http.put(`/api/Invoices/${id}/settle`);
}

export function refundInvoice(id) {
  return http.put(`/api/Invoices/${id}/refund`);
}
