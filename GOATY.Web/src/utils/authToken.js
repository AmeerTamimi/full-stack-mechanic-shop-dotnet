const ROLE_CLAIMS = [
  "role",
  "roles",
  "Role",
  "Roles",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role",
];

const EMAIL_CLAIMS = [
  "email",
  "Email",
  "sub",
  "name",
  "unique_name",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
];

const USER_ID_CLAIMS = [
  "sub",
  "nameid",
  "userId",
  "UserId",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
];

const EMPLOYEE_ID_CLAIMS = [
  "employeeId",
  "EmployeeId",
  "employeeID",
  "EmployeeID",
  "employee_id",
  "technicianId",
  "TechnicianId",
  "laborId",
  "LaborId",
];

function decodeBase64Url(value) {
  const base64 = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  const binary = atob(padded);
  const bytes = Uint8Array.from(binary, (character) => character.charCodeAt(0));

  return new TextDecoder().decode(bytes);
}

export function decodeJwtPayload(token) {
  if (!token || typeof token !== "string") {
    return null;
  }

  const [, payload] = token.split(".");

  if (!payload) {
    return null;
  }

  try {
    return JSON.parse(decodeBase64Url(payload));
  } catch {
    return null;
  }
}

function normalizeRoles(value) {
  if (!value) {
    return [];
  }

  if (Array.isArray(value)) {
    return value.flatMap(normalizeRoles);
  }

  return String(value)
    .split(",")
    .map((role) => role.trim())
    .filter(Boolean);
}

function readClaim(payload, claimNames) {
  for (const claimName of claimNames) {
    const value = payload?.[claimName];

    if (value !== undefined && value !== null) {
      return value;
    }
  }

  return null;
}

function normalizeGuidClaim(value) {
  if (Array.isArray(value)) {
    return value.map(normalizeGuidClaim).find(Boolean) ?? null;
  }

  const candidate = String(value ?? "").trim();

  if (!candidate) {
    return null;
  }

  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(candidate)
    ? candidate
    : null;
}

export function getJwtRoles(payload) {
  return ROLE_CLAIMS.flatMap((claimName) => normalizeRoles(payload?.[claimName]));
}

export function getJwtEmployeeId(payload) {
  return normalizeGuidClaim(readClaim(payload, EMPLOYEE_ID_CLAIMS));
}

export function getJwtUser(token) {
  const payload = decodeJwtPayload(token);

  if (!payload) {
    return null;
  }

  return {
    id: readClaim(payload, USER_ID_CLAIMS),
    email: readClaim(payload, EMAIL_CLAIMS),
    employeeId: getJwtEmployeeId(payload),
    roles: [...new Set(getJwtRoles(payload))],
    payload,
  };
}

export function hasAnyRole(userRoles = [], allowedRoles = []) {
  if (!allowedRoles.length) {
    return true;
  }

  const normalizedUserRoles = userRoles.map((role) => role.toLowerCase());

  return allowedRoles.some((role) => normalizedUserRoles.includes(role.toLowerCase()));
}
