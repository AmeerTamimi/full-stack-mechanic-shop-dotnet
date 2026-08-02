import { formatDateTime, formatMoney } from "@/utils/formatters";
import { asArray, readValue } from "@/utils/objectAccess";

export const INVOICE_STATUSES = {
  paid: 1,
  notPaid: 2,
  refunded: 3,
};

export const INVOICE_STATUS_OPTIONS = [
  { value: INVOICE_STATUSES.paid, label: "Paid", tone: "success" },
  { value: INVOICE_STATUSES.notPaid, label: "Not paid", tone: "warning" },
  { value: INVOICE_STATUSES.refunded, label: "Refunded", tone: "danger" },
];

export function getInvoiceId(invoice) {
  return readValue(invoice, "id", "Id");
}

export function getInvoiceWorkOrderId(invoice) {
  return readValue(invoice, "workOrderId", "WorkOrderId");
}

export function getInvoiceStatus(invoice) {
  return Number(readValue(invoice, "status", "Status", 0));
}

export function getInvoiceItems(invoice) {
  return asArray(readValue(invoice, "items", "Items", []));
}

export function getInvoiceStatusMeta(status) {
  return (
    INVOICE_STATUS_OPTIONS.find((option) => Number(option.value) === Number(status)) ?? {
      value: status,
      label: "Unknown",
      tone: "neutral",
    }
  );
}

export function getInvoiceStatusLabel(status) {
  return getInvoiceStatusMeta(status).label;
}

export function getInvoiceStatusTone(status) {
  return getInvoiceStatusMeta(status).tone;
}

export function canSettleInvoice(invoice) {
  return getInvoiceStatus(invoice) === INVOICE_STATUSES.notPaid;
}

export function canRefundInvoice(invoice) {
  return getInvoiceStatus(invoice) === INVOICE_STATUSES.paid;
}

export function formatInvoiceCode(id) {
  if (!id) {
    return "INV";
  }

  return `INV-${String(id).slice(0, 8).toUpperCase()}`;
}

export function formatInvoiceIssuedAt(invoice) {
  return formatDateTime(readValue(invoice, "issuedAt", "IssuedAt"));
}

export function formatInvoicePaidAt(invoice) {
  return formatDateTime(readValue(invoice, "paidAt", "PaidAt"), { fallback: "Not paid" });
}

export function formatInvoiceTotals(invoice) {
  return {
    subTotal: formatMoney(readValue(invoice, "subTotal", "SubTotal", 0)),
    discount: `${Number(readValue(invoice, "discount", "Discount", 0))}%`,
    tax: formatMoney(readValue(invoice, "tax", "Tax", 0)),
    total: formatMoney(readValue(invoice, "total", "Total", 0)),
  };
}

export function getInvoiceItemSourceLabel(item) {
  if (readValue(item, "repairTaskId", "RepairTaskId")) {
    return "Repair task";
  }

  if (readValue(item, "partId", "PartId")) {
    return "Part";
  }

  return "Line item";
}

function bytesFromBase64(base64) {
  const binary = window.atob(base64);
  const bytes = new Uint8Array(binary.length);

  for (let index = 0; index < binary.length; index += 1) {
    bytes[index] = binary.charCodeAt(index);
  }

  return bytes;
}

function getPdfBytes(content) {
  if (Array.isArray(content)) {
    return Uint8Array.from(content);
  }

  if (content instanceof Uint8Array) {
    return content;
  }

  return bytesFromBase64(String(content || ""));
}

export function downloadInvoicePdfFromData(data, fallbackName = "invoice.pdf") {
  const content = readValue(data, "content", "Content");
  const fileName = readValue(data, "fileName", "FileName", fallbackName) || fallbackName;
  const contentType = readValue(data, "contentType", "ContentType", "application/pdf");
  const bytes = getPdfBytes(content);
  const blob = new Blob([bytes], { type: contentType });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");

  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}
