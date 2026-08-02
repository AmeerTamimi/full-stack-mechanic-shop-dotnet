import { formatDate, formatMinutes } from "@/utils/formatters";
import { asArray, readValue } from "@/utils/objectAccess";
import {
  formatWorkOrderCode,
  getBayLabel,
  getEmployeeName,
  getStateLabel,
  getStateTone,
  WORK_ORDER_BAY_OPTIONS,
} from "@/utils/workOrders";

export const SCHEDULE_SLOT_MINUTES = 15;
export const SCHEDULE_ROW_HEIGHT = 28;
export const DEFAULT_VISIBLE_START_MINUTE = 7 * 60;
export const DEFAULT_VISIBLE_END_MINUTE = 19 * 60;

function padTimePart(value) {
  return String(value).padStart(2, "0");
}

function parseDate(value) {
  if (!value) return null;

  const parsed = new Date(value);

  return Number.isNaN(parsed.getTime()) ? null : parsed;
}

function addMinutes(date, minutes) {
  if (!date) return null;

  return new Date(date.getTime() + minutes * 60 * 1000);
}

function getMinutesFromDate(date) {
  if (!date) return 0;

  return date.getHours() * 60 + date.getMinutes();
}

function floorToSlot(minutes) {
  return Math.floor(minutes / SCHEDULE_SLOT_MINUTES) * SCHEDULE_SLOT_MINUTES;
}

function ceilToSlot(minutes) {
  return Math.ceil(minutes / SCHEDULE_SLOT_MINUTES) * SCHEDULE_SLOT_MINUTES;
}

function clampMinute(value) {
  return Math.min(24 * 60, Math.max(0, value));
}

function readTimeStampMinutes(value) {
  const numericValue = Number(value);

  if (Number.isFinite(numericValue) && numericValue > 0) {
    return numericValue;
  }

  const match = String(value ?? "").match(/\d+/);

  return match ? Number(match[0]) : 0;
}

export function getTodayDateInputValue(date = new Date()) {
  return `${date.getFullYear()}-${padTimePart(date.getMonth() + 1)}-${padTimePart(date.getDate())}`;
}

export function shiftDateInputValue(value, days) {
  const base = parseDate(`${value || getTodayDateInputValue()}T00:00:00`) ?? new Date();
  base.setDate(base.getDate() + days);

  return getTodayDateInputValue(base);
}

export function getDateTimeInputValue(date, fallbackHour = 9) {
  const baseDate = valueToInputDate(date);
  const today = getTodayDateInputValue();
  let hours = fallbackHour;
  let minutes = 0;

  if (baseDate === today) {
    const now = new Date();
    now.setMinutes(Math.ceil(now.getMinutes() / SCHEDULE_SLOT_MINUTES) * SCHEDULE_SLOT_MINUTES, 0, 0);
    now.setMinutes(now.getMinutes() + SCHEDULE_SLOT_MINUTES);

    if (getTodayDateInputValue(now) !== baseDate) {
      return `${getTodayDateInputValue(now)}T${padTimePart(now.getHours())}:${padTimePart(now.getMinutes())}`;
    }

    hours = now.getHours();
    minutes = now.getMinutes();
  }

  return `${baseDate}T${padTimePart(hours)}:${padTimePart(minutes)}`;
}

export function valueToInputDate(value) {
  if (!value) {
    return getTodayDateInputValue();
  }

  if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return value;
  }

  const parsed = parseDate(String(value).includes("T") ? value : `${value}T00:00:00`);

  return parsed ? getTodayDateInputValue(parsed) : getTodayDateInputValue();
}

export function formatScheduleDate(value) {
  return formatDate(value, { dateStyle: "full" });
}

export function formatScheduleTime(value, options = {}) {
  const { fallback = "Not set" } = options;
  const parsed = parseDate(value);

  if (!parsed) {
    return fallback;
  }

  return new Intl.DateTimeFormat("en-US", {
    hour: "numeric",
    minute: "2-digit",
  }).format(parsed);
}

export function formatMinuteOfDay(minutes) {
  const clamped = clampMinute(minutes);
  const hours = Math.floor(clamped / 60);
  const remainingMinutes = clamped % 60;
  const date = new Date();
  date.setHours(hours, remainingMinutes, 0, 0);

  return formatScheduleTime(date);
}

export function getScheduleDate(schedule) {
  return valueToInputDate(readValue(schedule, "date", "Date"));
}

export function getSlotWorkOrderId(slot) {
  return readValue(slot, "workOrderId", "WorkOrderId");
}

export function getSlotStartAt(slot) {
  return readValue(slot, "startAt", "StartAt");
}

export function getSlotEndAt(slot) {
  return readValue(slot, "endAt", "EndAt");
}

export function getSlotState(slot) {
  return Number(readValue(slot, "state", "State", 0));
}

export function getSlotVehicle(slot) {
  return readValue(slot, "vehicle", "Vehicle", "No vehicle");
}

export function getSlotEmployee(slot) {
  return readValue(slot, "employee", "Employee", null);
}

export function getSlotRepairTasks(slot) {
  return asArray(readValue(slot, "workOrderRepairTasks", "WorkOrderRepairTasks", []));
}

export function isSlotOccupied(slot) {
  return Boolean(readValue(slot, "isOccupied", "IsOccupied", false) && getSlotWorkOrderId(slot));
}

export function getRepairTaskMinutes(line) {
  const time = readTimeStampMinutes(readValue(line, "time", "Time", 0));
  const quantity = Number(readValue(line, "quantity", "Quantity", 1));

  return time * Math.max(1, quantity || 1);
}

export function getScheduleSlotDuration(slot) {
  const taskMinutes = getSlotRepairTasks(slot).reduce(
    (total, line) => total + getRepairTaskMinutes(line),
    0
  );

  if (taskMinutes > 0) {
    return taskMinutes;
  }

  const start = parseDate(getSlotStartAt(slot));
  const end = parseDate(getSlotEndAt(slot));
  const diffMinutes = start && end ? Math.round((end.getTime() - start.getTime()) / 60000) : 0;

  return Math.max(SCHEDULE_SLOT_MINUTES, diffMinutes);
}

export function formatScheduleSlotWindow(slot) {
  const start = parseDate(getSlotStartAt(slot));
  const end = addMinutes(start, getScheduleSlotDuration(slot));

  return `${formatScheduleTime(start)} - ${formatScheduleTime(end)}`;
}

export function getScheduleSlotLabel(slot) {
  return {
    code: formatWorkOrderCode(getSlotWorkOrderId(slot)),
    state: getStateLabel(getSlotState(slot)),
    tone: getStateTone(getSlotState(slot)),
    vehicle: getSlotVehicle(slot),
    employee: getEmployeeName(getSlotEmployee(slot)),
    tasks: getSlotRepairTasks(slot).length,
    duration: formatMinutes(getScheduleSlotDuration(slot)),
    window: formatScheduleSlotWindow(slot),
  };
}

export function normalizeScheduleBay(rawBay) {
  const bay = Number(readValue(rawBay, "bay", "Bay", 0));
  const slots = asArray(readValue(rawBay, "slots", "Slots", []));
  const bookedSlots = slots
    .filter(isSlotOccupied)
    .map((slot) => {
      const start = parseDate(getSlotStartAt(slot));
      const startMinutes = getMinutesFromDate(start);
      const durationMinutes = getScheduleSlotDuration(slot);

      return {
        ...slot,
        startMinutes,
        durationMinutes,
      };
    })
    .sort((first, second) => first.startMinutes - second.startMinutes);

  return {
    bay,
    label: getBayLabel(bay),
    slots,
    bookedSlots,
    availableSlots: slots.filter((slot) => !isSlotOccupied(slot)),
  };
}

export function normalizeSchedule(schedule) {
  const rawBays = asArray(readValue(schedule, "bays", "Bays", []));

  return WORK_ORDER_BAY_OPTIONS.map((bayOption) => {
    const matchingBay = rawBays.find(
      (rawBay) => Number(readValue(rawBay, "bay", "Bay")) === Number(bayOption.value)
    );

    return normalizeScheduleBay(matchingBay ?? { bay: bayOption.value, slots: [] });
  });
}

export function getScheduleRange(bays) {
  const bookedSlots = bays.flatMap((bay) => bay.bookedSlots);
  const earliestStart = bookedSlots.length
    ? Math.min(...bookedSlots.map((slot) => slot.startMinutes))
    : DEFAULT_VISIBLE_START_MINUTE;
  const latestEnd = bookedSlots.length
    ? Math.max(...bookedSlots.map((slot) => slot.startMinutes + slot.durationMinutes))
    : DEFAULT_VISIBLE_END_MINUTE;
  const startMinute = clampMinute(
    Math.min(DEFAULT_VISIBLE_START_MINUTE, floorToSlot(earliestStart) - 60)
  );
  const endMinute = clampMinute(Math.max(DEFAULT_VISIBLE_END_MINUTE, ceilToSlot(latestEnd) + 60));

  return {
    startMinute,
    endMinute: Math.max(endMinute, startMinute + 60),
  };
}

export function getScheduleTimeMarkers(range) {
  const markers = [];
  const start = Math.ceil(range.startMinute / 60) * 60;

  for (let minute = start; minute <= range.endMinute; minute += 60) {
    markers.push({
      minute,
      label: formatMinuteOfDay(minute),
      offset: getScheduleOffset(minute, range),
    });
  }

  return markers;
}

export function getScheduleOffset(minute, range) {
  return ((minute - range.startMinute) / SCHEDULE_SLOT_MINUTES) * SCHEDULE_ROW_HEIGHT;
}

export function getScheduleHeight(range) {
  return getScheduleOffset(range.endMinute, range);
}

export function getScheduleBlockStyle(slot, range) {
  const top = getScheduleOffset(slot.startMinutes, range);
  const height = Math.max(56, (slot.durationMinutes / SCHEDULE_SLOT_MINUTES) * SCHEDULE_ROW_HEIGHT - 8);

  return {
    top: `${Math.max(0, top)}px`,
    height: `${height}px`,
  };
}
