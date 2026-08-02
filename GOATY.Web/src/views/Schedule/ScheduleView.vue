<script setup>
import {
  CalendarDays,
  ChevronLeft,
  ChevronRight,
  ClipboardList,
  Clock3,
  LoaderCircle,
  Plus,
  RefreshCw,
  Search,
  UserCog,
  Wrench,
} from "@lucide/vue";
import { computed, onMounted, reactive, ref } from "vue";
import { RouterLink, useRoute, useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import EmptyState from "@/components/Shared/EmptyState.vue";
import ErrorState from "@/components/Shared/ErrorState.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { getDailySchedule } from "@/services/schedule.service";
import { getEmployees } from "@/services/employees.service";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMinutes } from "@/utils/formatters";
import { readValue } from "@/utils/objectAccess";
import {
  formatScheduleDate,
  getDateTimeInputValue,
  getScheduleBlockStyle,
  getScheduleDate,
  getScheduleHeight,
  getScheduleRange,
  getScheduleSlotLabel,
  getScheduleTimeMarkers,
  getTodayDateInputValue,
  normalizeSchedule,
  shiftDateInputValue,
  valueToInputDate,
} from "@/utils/schedule";
import { formatWorkOrderCode, getEmployeeName, WORK_ORDER_STATES } from "@/utils/workOrders";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();
const ui = useUiStore();

const schedule = ref(null);
const employees = ref([]);
const isLoading = ref(false);
const isReferencesLoading = ref(false);
const loadErrorMessage = ref("");

const filters = reactive({
  date: valueToInputDate(route.query.date),
  employeeId: route.query.employeeId?.toString() ?? "",
});

const bayColumns = computed(() => normalizeSchedule(schedule.value));
const bookedSlots = computed(() => {
  return bayColumns.value
    .flatMap((bay) => bay.bookedSlots.map((slot) => ({ ...slot, bayLabel: bay.label })))
    .sort((first, second) => first.startMinutes - second.startMinutes);
});
const activeSlots = computed(() => {
  return bookedSlots.value.filter((slot) => {
    const state = Number(readValue(slot, "state", "State", 0));

    return state === WORK_ORDER_STATES.scheduled || state === WORK_ORDER_STATES.inProgress;
  });
});
const inProgressCount = computed(() => {
  return bookedSlots.value.filter((slot) => Number(readValue(slot, "state", "State", 0)) === WORK_ORDER_STATES.inProgress).length;
});
const totalWorkloadMinutes = computed(() => {
  return bookedSlots.value.reduce((total, slot) => total + Number(slot.durationMinutes || 0), 0);
});
const scheduleRange = computed(() => getScheduleRange(bayColumns.value));
const timeMarkers = computed(() => getScheduleTimeMarkers(scheduleRange.value));
const boardHeight = computed(() => getScheduleHeight(scheduleRange.value));
const scheduleDateLabel = computed(() => {
  return formatScheduleDate(schedule.value ? getScheduleDate(schedule.value) : filters.date);
});
const technicianOptions = computed(() => {
  return employees.value
    .filter((employee) => Number(readValue(employee, "role", "Role")) === 2)
    .map((employee) => ({
      id: readValue(employee, "id", "Id"),
      label: getEmployeeName(employee),
    }));
});
const selectedTechnicianLabel = computed(() => {
  if (!filters.employeeId) return "All technicians";

  return technicianOptions.value.find((employee) => employee.id === filters.employeeId)?.label ?? "Selected technician";
});

function getScheduleQuery() {
  return {
    Day: filters.date || undefined,
    EmployeeId: filters.employeeId || undefined,
  };
}

function getRouteQuery() {
  return {
    date: filters.date || undefined,
    employeeId: filters.employeeId || undefined,
  };
}

async function syncRouteQuery() {
  await router.replace({
    name: "schedule",
    query: getRouteQuery(),
  });
}

async function loadSchedule() {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getDailySchedule(getScheduleQuery());
    schedule.value = data;
    filters.date = getScheduleDate(data);
    await syncRouteQuery();
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Unable to load the schedule for this day."
    );
  } finally {
    isLoading.value = false;
  }
}

async function loadReferences() {
  if (!auth.isManager) return;

  isReferencesLoading.value = true;

  try {
    const { data } = await getEmployees({ Page: 1, PageSize: 100 });
    employees.value = normalizePaginatedResponse(data, {
      page: 1,
      pageSize: 100,
    }).items;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to load technician filters."),
      "Schedule filters unavailable"
    );
  } finally {
    isReferencesLoading.value = false;
  }
}

async function applyFilters() {
  await loadSchedule();
}

async function moveDay(days) {
  filters.date = shiftDateInputValue(filters.date, days);
  await loadSchedule();
}

async function goToToday() {
  filters.date = getTodayDateInputValue();
  await loadSchedule();
}

function getCreateRoute(bay) {
  return {
    name: "work-order-create",
    query: {
      startTime: getDateTimeInputValue(filters.date),
      bay: bay.bay,
    },
  };
}

function getGlobalCreateRoute() {
  return {
    name: "work-order-create",
    query: {
      startTime: getDateTimeInputValue(filters.date),
    },
  };
}

function getWorkOrderRoute(slot) {
  return {
    name: "work-order-details",
    params: {
      id: readValue(slot, "workOrderId", "WorkOrderId"),
    },
  };
}

function getBlockToneClass(slot) {
  const label = getScheduleSlotLabel(slot);

  return `schedule-block--${label.tone}`;
}

function getBayHelperLabel(bay) {
  if (!bay.bookedSlots.length) {
    return "No booked work in the visible window";
  }

  return `${bay.bookedSlots.length} booked / ${bay.availableSlots.length} open slots`;
}

onMounted(async () => {
  await loadReferences();
  await loadSchedule();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Operations"
      title="Schedule"
      subtitle="Daily bay plan for work orders, technicians, and shop capacity."
      :icon="CalendarDays"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh schedule"
          @click="loadSchedule"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton v-if="auth.isManager" :to="getGlobalCreateRoute()">
          <Plus :size="18" />
          <span>New work order</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Schedule summary">
      <SummaryCard label="Schedule date" :value="scheduleDateLabel" />
      <SummaryCard label="Booked jobs" :value="bookedSlots.length" />
      <SummaryCard label="Active jobs" :value="activeSlots.length" />
      <SummaryCard label="Workload" :value="formatMinutes(totalWorkloadMinutes)" />
    </SummaryGrid>

    <ContentPanel class="schedule-controls">
      <form class="schedule-filter-form" @submit.prevent="applyFilters">
        <div class="day-steppers" aria-label="Schedule day navigation">
          <ActionButton
            variant="secondary"
            icon-only
            type="button"
            :disabled="isLoading"
            aria-label="Previous day"
            @click="moveDay(-1)"
          >
            <ChevronLeft :size="18" />
          </ActionButton>

          <label class="date-control">
            <CalendarDays :size="17" />
            <span>Date</span>
            <input v-model="filters.date" type="date" :disabled="isLoading" />
          </label>

          <ActionButton
            variant="secondary"
            icon-only
            type="button"
            :disabled="isLoading"
            aria-label="Next day"
            @click="moveDay(1)"
          >
            <ChevronRight :size="18" />
          </ActionButton>
        </div>

        <label v-if="auth.isManager" class="select-control">
          <UserCog :size="17" />
          <span>Technician</span>
          <select
            v-model="filters.employeeId"
            :disabled="isLoading || isReferencesLoading"
          >
            <option value="">All technicians</option>
            <option
              v-for="technician in technicianOptions"
              :key="technician.id"
              :value="technician.id"
            >
              {{ technician.label }}
            </option>
          </select>
        </label>

        <div class="schedule-filter-actions">
          <StatusChip :label="selectedTechnicianLabel" tone="neutral" :icon="UserCog" />

          <ActionButton type="submit" variant="secondary" :disabled="isLoading">
            <LoaderCircle v-if="isLoading" class="spinning" :size="18" />
            <Search v-else :size="18" />
            <span>Apply</span>
          </ActionButton>

          <ActionButton variant="ghost" type="button" :disabled="isLoading" @click="goToToday">
            <Clock3 :size="18" />
            <span>Today</span>
          </ActionButton>
        </div>
      </form>
    </ContentPanel>

    <ErrorState
      v-if="loadErrorMessage"
      title="Unable to load schedule"
      :message="loadErrorMessage"
      @retry="loadSchedule"
    />

    <LoadingState v-else-if="isLoading && !schedule" message="Loading schedule..." />

    <ContentPanel v-else class="schedule-board-panel">
      <div class="board-heading">
        <div>
          <span class="board-kicker">{{ scheduleDateLabel }}</span>
          <h2>Bay timeline</h2>
        </div>
        <div class="board-heading__chips">
          <StatusChip :label="`${inProgressCount} in progress`" tone="info" />
          <StatusChip :label="`${bookedSlots.length} booked`" tone="service" :icon="Wrench" />
        </div>
      </div>

      <div
        class="schedule-board"
        :style="{ '--schedule-height': `${boardHeight}px` }"
        aria-label="Daily bay schedule"
      >
        <aside class="time-axis" aria-label="Time axis">
          <span
            v-for="marker in timeMarkers"
            :key="marker.minute"
            class="time-marker"
            :style="{ top: `${marker.offset}px` }"
          >
            {{ marker.label }}
          </span>
        </aside>

        <section v-for="bay in bayColumns" :key="bay.bay" class="bay-column">
          <header class="bay-column__header">
            <div>
              <strong>{{ bay.label }}</strong>
              <small>{{ getBayHelperLabel(bay) }}</small>
            </div>
            <ActionButton
              v-if="auth.isManager"
              variant="secondary"
              size="sm"
              icon-only
              :to="getCreateRoute(bay)"
              aria-label="Create work order in this bay"
            >
              <Plus :size="16" />
            </ActionButton>
          </header>

          <div class="bay-lane">
            <span
              v-for="marker in timeMarkers"
              :key="`${bay.bay}-${marker.minute}`"
              class="lane-line"
              :style="{ top: `${marker.offset}px` }"
            ></span>

            <RouterLink
              v-for="slot in bay.bookedSlots"
              :key="readValue(slot, 'workOrderId', 'WorkOrderId')"
              class="schedule-block"
              :class="getBlockToneClass(slot)"
              :style="getScheduleBlockStyle(slot, scheduleRange)"
              :to="getWorkOrderRoute(slot)"
            >
              <div class="schedule-block__topline">
                <strong>{{ getScheduleSlotLabel(slot).code }}</strong>
                <StatusChip
                  :label="getScheduleSlotLabel(slot).state"
                  :tone="getScheduleSlotLabel(slot).tone"
                  size="sm"
                />
              </div>
              <span class="schedule-block__vehicle">{{ getScheduleSlotLabel(slot).vehicle }}</span>
              <span class="schedule-block__meta">
                {{ getScheduleSlotLabel(slot).window }}
                /
                {{ getScheduleSlotLabel(slot).duration }}
              </span>
              <span class="schedule-block__meta">
                {{ getScheduleSlotLabel(slot).employee }}
                /
                {{ getScheduleSlotLabel(slot).tasks }} tasks
              </span>
            </RouterLink>

            <div v-if="!bay.bookedSlots.length" class="bay-empty">
              <ClipboardList :size="22" />
              <strong>Open</strong>
              <span>No work orders in this visible window.</span>
            </div>
          </div>
        </section>
      </div>

      <div class="schedule-list" aria-label="Mobile schedule list">
        <article v-for="slot in bookedSlots" :key="`${slot.bay}-${readValue(slot, 'workOrderId', 'WorkOrderId')}`" class="schedule-list-item">
          <div class="schedule-list-item__main">
            <span class="schedule-list-item__bay">{{ slot.bayLabel }}</span>
            <strong>{{ formatWorkOrderCode(readValue(slot, "workOrderId", "WorkOrderId")) }}</strong>
            <small>{{ getScheduleSlotLabel(slot).vehicle }}</small>
          </div>
          <div class="schedule-list-item__meta">
            <StatusChip
              :label="getScheduleSlotLabel(slot).state"
              :tone="getScheduleSlotLabel(slot).tone"
            />
            <span>{{ getScheduleSlotLabel(slot).window }}</span>
            <span>{{ getScheduleSlotLabel(slot).employee }}</span>
          </div>
          <ActionButton
            variant="secondary"
            size="sm"
            :to="getWorkOrderRoute(slot)"
          >
            <ClipboardList :size="16" />
            <span>Open</span>
          </ActionButton>
        </article>

        <EmptyState
          v-if="!bookedSlots.length"
          title="No work orders on this day"
          message="Choose another date or create a new work order for this schedule."
        >
          <template #icon>
            <CalendarDays :size="28" />
          </template>
          <template v-if="auth.isManager" #action>
            <ActionButton :to="getGlobalCreateRoute()">
              <Plus :size="18" />
              <span>Create work order</span>
            </ActionButton>
          </template>
        </EmptyState>
      </div>
    </ContentPanel>
  </PageShell>
</template>

<style scoped>
.schedule-controls {
  margin-bottom: 18px;
}

.schedule-filter-form {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.day-steppers,
.schedule-filter-actions {
  display: flex;
  align-items: center;
  gap: 9px;
  min-width: 0;
}

.date-control,
.select-control {
  display: inline-flex;
  align-items: center;
  gap: 9px;
  min-height: 44px;
  min-width: 0;
  padding: 0 12px;
  color: #64748b;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-weight: 850;
}

.date-control input,
.select-control select {
  min-width: 160px;
  height: 40px;
  color: #111827;
  background: transparent;
  border: 0;
  outline: none;
  font: inherit;
}

.select-control select {
  min-width: 220px;
}

.board-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 18px;
  margin-bottom: 18px;
}

.board-kicker {
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  letter-spacing: 0;
  text-transform: uppercase;
}

.board-heading h2 {
  margin: 4px 0 0;
  color: #111827;
  font-size: 22px;
}

.board-heading__chips {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  justify-content: flex-end;
}

.schedule-board {
  display: grid;
  grid-template-columns: 74px repeat(4, minmax(230px, 1fr));
  align-items: stretch;
  overflow-x: auto;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #fff;
}

.time-axis {
  position: relative;
  min-height: var(--schedule-height);
  margin-top: 76px;
  background: #f8fafc;
  border-right: 1px solid #e5e7eb;
}

.time-marker {
  position: absolute;
  right: 10px;
  transform: translateY(-50%);
  color: #64748b;
  font-size: 12px;
  font-weight: 850;
  white-space: nowrap;
}

.bay-column {
  min-width: 230px;
  border-right: 1px solid #e5e7eb;
}

.bay-column:last-child {
  border-right: 0;
}

.bay-column__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  min-height: 76px;
  padding: 14px;
  background:
    linear-gradient(180deg, #f8fafc, #fff),
    repeating-linear-gradient(90deg, transparent 0 20px, rgba(15, 23, 42, 0.04) 20px 21px);
  border-bottom: 1px solid #e5e7eb;
}

.bay-column__header strong,
.bay-column__header small {
  display: block;
}

.bay-column__header strong {
  color: #111827;
  font-size: 16px;
}

.bay-column__header small {
  margin-top: 4px;
  color: #64748b;
  font-size: 12px;
  font-weight: 750;
  line-height: 1.35;
}

.bay-lane {
  position: relative;
  min-height: var(--schedule-height);
  background:
    repeating-linear-gradient(
      180deg,
      rgba(148, 163, 184, 0.08) 0,
      rgba(148, 163, 184, 0.08) 1px,
      transparent 1px,
      transparent 28px
    ),
    linear-gradient(180deg, #fff, #f8fafc);
}

.lane-line {
  position: absolute;
  right: 0;
  left: 0;
  height: 1px;
  background: rgba(15, 23, 42, 0.12);
}

.schedule-block {
  position: absolute;
  right: 10px;
  left: 10px;
  z-index: 2;
  display: grid;
  align-content: start;
  gap: 5px;
  min-height: 56px;
  padding: 10px;
  overflow: hidden;
  color: #111827;
  text-decoration: none;
  background: #fff7ed;
  border: 1px solid rgba(245, 158, 11, 0.42);
  border-left: 5px solid #f59e0b;
  border-radius: 8px;
  box-shadow: 0 14px 24px rgba(15, 23, 42, 0.12);
}

.schedule-block:hover {
  transform: translateY(-1px);
  box-shadow: 0 18px 30px rgba(15, 23, 42, 0.16);
}

.schedule-block--info {
  background: #eff6ff;
  border-color: rgba(59, 130, 246, 0.42);
  border-left-color: #3b82f6;
}

.schedule-block--success {
  background: #ecfdf5;
  border-color: rgba(34, 197, 94, 0.42);
  border-left-color: #22c55e;
}

.schedule-block--danger {
  background: #fff1f2;
  border-color: rgba(244, 63, 94, 0.42);
  border-left-color: #f43f5e;
}

.schedule-block--neutral {
  background: #f8fafc;
  border-color: rgba(100, 116, 139, 0.35);
  border-left-color: #64748b;
}

.schedule-block__topline {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 8px;
  min-width: 0;
}

.schedule-block__topline strong,
.schedule-block__vehicle,
.schedule-block__meta {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.schedule-block__topline strong {
  font-size: 14px;
}

.schedule-block__vehicle {
  font-size: 13px;
  font-weight: 850;
}

.schedule-block__meta {
  color: #64748b;
  font-size: 12px;
  font-weight: 750;
}

.bay-empty {
  position: absolute;
  top: 50%;
  right: 16px;
  left: 16px;
  display: grid;
  place-items: center;
  gap: 7px;
  padding: 18px 12px;
  color: #64748b;
  text-align: center;
  transform: translateY(-50%);
  border: 1px dashed #cbd5e1;
  border-radius: 8px;
}

.bay-empty strong {
  color: #111827;
}

.bay-empty span {
  font-size: 12px;
  font-weight: 750;
}

.schedule-list {
  display: none;
}

.schedule-list-item {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto auto;
  align-items: center;
  gap: 12px;
  padding: 14px;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.schedule-list-item__main,
.schedule-list-item__meta {
  display: grid;
  gap: 5px;
  min-width: 0;
}

.schedule-list-item__bay {
  color: #f59e0b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.schedule-list-item__main strong,
.schedule-list-item__main small,
.schedule-list-item__meta span {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.schedule-list-item__meta {
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

@media (max-width: 980px) {
  .schedule-filter-form,
  .day-steppers,
  .schedule-filter-actions,
  .date-control,
  .select-control {
    align-items: stretch;
    width: 100%;
  }

  .schedule-filter-form,
  .schedule-filter-actions {
    flex-direction: column;
  }

  .day-steppers {
    display: grid;
    grid-template-columns: auto minmax(0, 1fr) auto;
  }

  .date-control input,
  .select-control select {
    width: 100%;
    min-width: 0;
  }

  .board-heading {
    flex-direction: column;
  }

  .board-heading__chips {
    justify-content: flex-start;
  }

  .schedule-board {
    display: none;
  }

  .schedule-list {
    display: grid;
    gap: 12px;
  }

  .schedule-list-item {
    grid-template-columns: 1fr;
  }
}
</style>
