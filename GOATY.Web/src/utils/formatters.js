export function formatMoney(value, options = {}) {
  const { currency = "USD", locale = "en-US", maximumFractionDigits = 2 } = options;

  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency,
    maximumFractionDigits,
  }).format(Number(value || 0));
}

export function formatPercent(value, options = {}) {
  const { fractionDigits = 1 } = options;

  return `${Number(value || 0).toFixed(fractionDigits)}%`;
}

export function formatDate(value, options = {}) {
  const { fallback = "Not set", locale = "en-US", dateStyle } = options;

  if (!value) {
    return fallback;
  }

  const parsedDate = new Date(String(value).includes("T") ? value : `${value}T00:00:00`);

  if (Number.isNaN(parsedDate.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat(
    locale,
    dateStyle
      ? { dateStyle }
      : {
          weekday: "short",
          month: "short",
          day: "numeric",
          year: "numeric",
        }
  ).format(parsedDate);
}

export function formatDateTime(value, options = {}) {
  const { fallback = "Not set", locale = "en-US" } = options;

  if (!value) {
    return fallback;
  }

  const parsedDate = new Date(value);

  if (Number.isNaN(parsedDate.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat(locale, {
    month: "short",
    day: "numeric",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  }).format(parsedDate);
}

export function formatMinutes(value) {
  const minutes = Number(value || 0);

  if (minutes <= 0) {
    return "0 min";
  }

  if (minutes < 60) {
    return `${minutes} min`;
  }

  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;

  if (!remainingMinutes) {
    return `${hours} hr`;
  }

  return `${hours} hr ${remainingMinutes} min`;
}
