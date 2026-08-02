import { asArray, readValue } from "@/utils/objectAccess";

export function normalizePaginatedResponse(data, defaults = {}) {
  const fallbackPage = Number(defaults.page ?? 1);
  const fallbackPageSize = Number(defaults.pageSize ?? 10);
  const rawItems = Array.isArray(data) ? data : readValue(data, "items", "Items", []);
  const items = asArray(rawItems);
  const page = Number(readValue(data, "page", "Page", fallbackPage)) || fallbackPage;
  const pageSize = Number(readValue(data, "pageSize", "PageSize", fallbackPageSize)) || fallbackPageSize;
  const totalItems = Number(readValue(data, "totalItems", "TotalItems", items.length)) || 0;
  const rawTotalPages = Number(readValue(data, "totalPages", "TotalPages", 0)) || 0;
  const computedTotalPages = pageSize > 0 ? Math.ceil(totalItems / pageSize) : 1;
  const totalPages = rawTotalPages || computedTotalPages || 1;

  return {
    items,
    page,
    pageSize,
    totalItems,
    totalPages: Math.max(1, totalPages),
  };
}

function flattenErrorMessages(errors) {
  if (!errors || typeof errors !== "object") {
    return [];
  }

  return Object.values(errors)
    .flatMap((value) => (Array.isArray(value) ? value : [value]))
    .filter(Boolean)
    .map(String);
}

export function getBackendErrorMessage(error, fallbackMessage = "Something went wrong. Please try again.") {
  const data = error?.response?.data;

  if (!data) {
    return fallbackMessage;
  }

  if (typeof data === "string") {
    return data;
  }

  const validationMessages = flattenErrorMessages(data.errors ?? data.Errors);
  if (validationMessages.length) {
    return validationMessages.join(" ");
  }

  return (
    data.detail ??
    data.Detail ??
    data.title ??
    data.Title ??
    data.message ??
    data.Message ??
    fallbackMessage
  );
}

export function getBackendErrorTitle(error, fallbackTitle = "Something went wrong", options = {}) {
  const { conflictTitle = "Conflict" } = options;
  const status = error?.response?.status;

  if (status === 400) return "Check the form";
  if (status === 401) return "Session expired";
  if (status === 403) return "Access denied";
  if (status === 404) return "Not found";
  if (status === 409) return conflictTitle;
  if (!error?.response) return "Connection failed";

  return fallbackTitle;
}

export function isUnauthorizedError(error) {
  return error?.response?.status === 401;
}

export function isForbiddenError(error) {
  return error?.response?.status === 403;
}
