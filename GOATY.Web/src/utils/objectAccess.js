export function toPascalCase(key) {
  if (!key) return key;

  return `${key.charAt(0).toUpperCase()}${key.slice(1)}`;
}

export function readValue(source, camelKey, pascalKey = toPascalCase(camelKey), fallback = "") {
  if (!source || typeof source !== "object") {
    return fallback;
  }

  const camelValue = source[camelKey];
  if (camelValue !== undefined && camelValue !== null) {
    return camelValue;
  }

  const pascalValue = source[pascalKey];
  if (pascalValue !== undefined && pascalValue !== null) {
    return pascalValue;
  }

  return fallback;
}

export function readFirstValue(source, keys, fallback = "") {
  if (!source || typeof source !== "object") {
    return fallback;
  }

  for (const key of keys) {
    const value = source[key];

    if (value !== undefined && value !== null) {
      return value;
    }
  }

  return fallback;
}

export function asArray(value) {
  return Array.isArray(value) ? value : [];
}

export function matchesSearch(values, searchTerm) {
  const normalizedSearch = String(searchTerm || "").trim().toLowerCase();

  if (!normalizedSearch) {
    return true;
  }

  return values
    .filter((value) => value !== undefined && value !== null)
    .some((value) => String(value).toLowerCase().includes(normalizedSearch));
}
