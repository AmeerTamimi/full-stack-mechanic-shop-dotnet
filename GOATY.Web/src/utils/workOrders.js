import { formatDateTime, formatMinutes, formatMoney } from "@/utils/formatters";
import { asArray, readValue } from "@/utils/objectAccess";

export const WORK_ORDER_STATES = {
  scheduled: 1,
  inProgress: 2,
  completed: 3,
  cancelled: 4,
};

export const WORK_ORDER_STATE_OPTIONS = [
  { value: WORK_ORDER_STATES.scheduled, label: "Scheduled", tone: "warning" },
  { value: WORK_ORDER_STATES.inProgress, label: "In progress", tone: "info" },
  { value: WORK_ORDER_STATES.completed, label: "Completed", tone: "success" },
  { value: WORK_ORDER_STATES.cancelled, label: "Cancelled", tone: "danger" },
];

export const WORK_ORDER_BAYS = {
  A: 1,
  B: 2,
  C: 3,
  D: 4,
};

export const WORK_ORDER_BAY_OPTIONS = [
  { value: WORK_ORDER_BAYS.A, label: "Bay A" },
  { value: WORK_ORDER_BAYS.B, label: "Bay B" },
  { value: WORK_ORDER_BAYS.C, label: "Bay C" },
  { value: WORK_ORDER_BAYS.D, label: "Bay D" },
];

export const WORK_ORDER_SORT_OPTIONS = [
  { value: "createdAt:desc", label: "Newest first", column: "createdAt", direction: "desc" },
  { value: "startAt:asc", label: "Start time", column: "startAt", direction: "asc" },
  { value: "endAt:asc", label: "End time", column: "endAt", direction: "asc" },
  { value: "state:asc", label: "State", column: "state", direction: "asc" },
  { value: "spot:asc", label: "Bay", column: "spot", direction: "asc" },
];

const VALID_STATE_TRANSITIONS = {
  [WORK_ORDER_STATES.scheduled]: [WORK_ORDER_STATES.inProgress, WORK_ORDER_STATES.cancelled],
  [WORK_ORDER_STATES.inProgress]: [WORK_ORDER_STATES.completed, WORK_ORDER_STATES.cancelled],
  [WORK_ORDER_STATES.completed]: [],
  [WORK_ORDER_STATES.cancelled]: [],
};

export function getWorkOrderId(workOrder) {
  return readValue(workOrder, "id", "Id");
}

export function getWorkOrderState(workOrder) {
  return Number(readValue(workOrder, "state", "State", 0));
}

export function getStateMeta(state) {
  return (
    WORK_ORDER_STATE_OPTIONS.find((option) => Number(option.value) === Number(state)) ?? {
      value: state,
      label: "Unknown",
      tone: "neutral",
    }
  );
}

export function getStateLabel(state) {
  return getStateMeta(state).label;
}

export function getStateTone(state) {
  return getStateMeta(state).tone;
}

export function getAllowedNextStates(state) {
  return VALID_STATE_TRANSITIONS[Number(state)] ?? [];
}

export function isScheduled(workOrder) {
  return getWorkOrderState(workOrder) === WORK_ORDER_STATES.scheduled;
}

export function getBayLabel(bay) {
  return (
    WORK_ORDER_BAY_OPTIONS.find((option) => Number(option.value) === Number(bay))?.label ??
    `Bay ${bay || "-"}`
  );
}

export function getCustomer(workOrder) {
  return readValue(workOrder, "customer", "Customer", null);
}

export function getVehicle(workOrder) {
  return readValue(workOrder, "vehicle", "Vehicle", null);
}

export function getEmployee(workOrder) {
  return readValue(workOrder, "employee", "Employee", null);
}

export function getCustomerName(customer) {
  const fullName = readValue(customer, "fullName", "FullName");

  if (fullName) {
    return fullName;
  }

  return `${readValue(customer, "firstName", "FirstName")} ${readValue(
    customer,
    "lastName",
    "LastName"
  )}`.trim() || "No customer";
}

export function getEmployeeName(employee) {
  const fullName = readValue(employee, "fullName", "FullName");

  if (fullName) {
    return fullName;
  }

  return `${readValue(employee, "firstName", "FirstName")} ${readValue(
    employee,
    "lastName",
    "LastName"
  )}`.trim() || "Unassigned";
}

export function getEmployeeEmail(employee) {
  return readValue(employee, "email", "Email");
}

export function getVehicleLabel(vehicle) {
  if (!vehicle) {
    return "No vehicle";
  }

  const vehicleInfo = readValue(vehicle, "vehicleInfo", "VehicleInfo");

  if (vehicleInfo) {
    return vehicleInfo;
  }

  return `${readValue(vehicle, "brand", "Brand")} ${readValue(vehicle, "model", "Model")} ${readValue(
    vehicle,
    "year",
    "Year"
  )}`.trim();
}

export function getVehiclePlate(vehicle) {
  return readValue(vehicle, "licensePlate", "LicensePlate");
}

export function getCustomerVehicles(customer) {
  return asArray(readValue(customer, "vehicles", "Vehicles", []));
}

export function getWorkOrderRepairTaskLines(workOrder) {
  return asArray(readValue(workOrder, "repairTasks", "RepairTasks", []));
}

export function getRepairTaskFromLine(line) {
  return readValue(line, "repairTask", "RepairTask", null);
}

export function getRepairTaskId(repairTask) {
  return readValue(repairTask, "id", "Id");
}

export function getRepairTaskName(repairTask) {
  return readValue(repairTask, "name", "Name", "Unnamed task");
}

export function getWorkOrderTaskName(line) {
  return getRepairTaskName(getRepairTaskFromLine(line));
}

export function getWorkOrderTaskCost(line) {
  return Number(readValue(line, "cost", "Cost", 0));
}

export function getWorkOrderTaskTime(line) {
  return Number(readValue(line, "time", "Time", 0));
}

export function formatWorkOrderCode(id) {
  if (!id) {
    return "WO";
  }

  return `WO-${String(id).slice(0, 8).toUpperCase()}`;
}

export function formatWorkOrderWindow(workOrder) {
  const startTime = readValue(workOrder, "startTime", "StartTime");
  const endTime = readValue(workOrder, "endTime", "EndTime");

  return `${formatDateTime(startTime)} - ${formatDateTime(endTime, { fallback: "not set" })}`;
}

export function formatWorkOrderTotals(workOrder) {
  return {
    totalTime: formatMinutes(readValue(workOrder, "totalTime", "TotalTime", 0)),
    totalCost: formatMoney(readValue(workOrder, "totalCost", "TotalCost", 0)),
    partsCost: formatMoney(readValue(workOrder, "totalPartsCost", "TotalPartsCost", 0)),
    laborCost: formatMoney(readValue(workOrder, "totalTechniciansCost", "TotalTechniciansCost", 0)),
  };
}

export function toDateTimeLocalValue(value) {
  if (!value) return "";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}
