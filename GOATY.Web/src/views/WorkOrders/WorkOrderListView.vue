<script setup>
import {
  CalendarDays,
  Car,
  ClipboardList,
  Eye,
  Plus,
  RefreshCw,
  Search,
  Trash2,
  UserCog,
  Wrench,
  X,
} from "@lucide/vue";
import { computed, onMounted, reactive, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import EntityFilterBar from "@/components/Shared/EntityFilterBar.vue";
import EntityListShell from "@/components/Shared/EntityListShell.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { getCustomers } from "@/services/customers.service";
import { getEmployees } from "@/services/employees.service";
import { deleteWorkOrder, getWorkOrders } from "@/services/workOrders.service";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMinutes } from "@/utils/formatters";
import { readValue } from "@/utils/objectAccess";
import {
  formatWorkOrderCode,
  formatWorkOrderWindow,
  getBayLabel,
  getCustomer,
  getCustomerName,
  getCustomerVehicles,
  getEmployee,
  getEmployeeName,
  getStateLabel,
  getStateTone,
  getVehicle,
  getVehicleLabel,
  getVehiclePlate,
  getWorkOrderId,
  getWorkOrderRepairTaskLines,
  getWorkOrderState,
  WORK_ORDER_BAY_OPTIONS,
  WORK_ORDER_SORT_OPTIONS,
  WORK_ORDER_STATE_OPTIONS,
  WORK_ORDER_STATES,
} from "@/utils/workOrders";

const auth = useAuthStore();
const ui = useUiStore();
const workOrders = ref([]);
const customers = ref([]);
const employees = ref([]);
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingWorkOrderId = ref(null);
const loadErrorMessage = ref("");

const filters = reactive({
  searchTerm: "",
  state: "",
  bay: "",
  laborId: "",
  vehicleId: "",
  startDateFrom: "",
  startDateTo: "",
  sort: "createdAt:desc",
});

const hasWorkOrders = computed(() => workOrders.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
const scheduledCount = computed(
  () => workOrders.value.filter((workOrder) => getWorkOrderState(workOrder) === WORK_ORDER_STATES.scheduled).length
);
const inProgressCount = computed(
  () => workOrders.value.filter((workOrder) => getWorkOrderState(workOrder) === WORK_ORDER_STATES.inProgress).length
);
const completedCount = computed(
  () => workOrders.value.filter((workOrder) => getWorkOrderState(workOrder) === WORK_ORDER_STATES.completed).length
);
const resultLabel = computed(() => `${totalItems.value} total`);
const technicianOptions = computed(() => {
  return employees.value
    .filter((employee) => Number(readValue(employee, "role", "Role")) === 2)
    .map((employee) => ({
      id: readValue(employee, "id", "Id"),
      label: getEmployeeName(employee),
    }));
});
const vehicleOptions = computed(() => {
  return customers.value.flatMap((customer) =>
    getCustomerVehicles(customer).map((vehicle) => ({
      id: readValue(vehicle, "id", "Id"),
      label: `${getVehicleLabel(vehicle)} - ${getCustomerName(customer)}`,
    }))
  );
});
const hasActiveFilters = computed(() => {
  return Object.entries(filters).some(([key, value]) => key !== "sort" && Boolean(value));
});

function getSortParts() {
  const option =
    WORK_ORDER_SORT_OPTIONS.find((sortOption) => sortOption.value === filters.sort) ??
    WORK_ORDER_SORT_OPTIONS[0];

  return {
    SortColumn: option.column,
    SortDirection: option.direction,
  };
}

function getWorkOrderQuery(targetPage) {
  return {
    Page: targetPage,
    PageSize: pageSize.value,
    SearchTerm: filters.searchTerm.trim() || undefined,
    State: filters.state || undefined,
    Bay: filters.bay || undefined,
    LaborId: filters.laborId || undefined,
    VehicleId: filters.vehicleId || undefined,
    StartDateFrom: filters.startDateFrom ? `${filters.startDateFrom}T00:00:00` : undefined,
    StartDateTo: filters.startDateTo ? `${filters.startDateTo}T23:59:59` : undefined,
    ...getSortParts(),
  };
}

async function loadWorkOrders(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getWorkOrders(getWorkOrderQuery(targetPage));
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    workOrders.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading work orders."
    );
  } finally {
    isLoading.value = false;
  }
}

async function loadFilterReferences() {
  if (!auth.isManager) return;

  try {
    const [customersResponse, employeesResponse] = await Promise.all([
      getCustomers({ Page: 1, PageSize: 100 }),
      getEmployees({ Page: 1, PageSize: 100 }),
    ]);

    customers.value = normalizePaginatedResponse(customersResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
    employees.value = normalizePaginatedResponse(employeesResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to load filter options."),
      "Filters unavailable"
    );
  }
}

async function applyFilters() {
  await loadWorkOrders(1);
}

async function resetFilters() {
  filters.searchTerm = "";
  filters.state = "";
  filters.bay = "";
  filters.laborId = "";
  filters.vehicleId = "";
  filters.startDateFrom = "";
  filters.startDateTo = "";
  filters.sort = "createdAt:desc";
  await loadWorkOrders(1);
}

async function handleDelete(workOrder) {
  const workOrderId = getWorkOrderId(workOrder);

  if (!workOrderId) return;

  const shouldDelete = await ui.confirm({
    title: "Delete work order?",
    message: `This will permanently remove ${formatWorkOrderCode(workOrderId)}.`,
    confirmText: "Delete work order",
    cancelText: "Keep work order",
    variant: "danger",
  });

  if (!shouldDelete) return;

  deletingWorkOrderId.value = workOrderId;

  try {
    await deleteWorkOrder(workOrderId);
    ui.showSuccessToast(`${formatWorkOrderCode(workOrderId)} was deleted.`, "Work order deleted");

    if (workOrders.value.length === 1 && page.value > 1) {
      await loadWorkOrders(page.value - 1);
    } else {
      await loadWorkOrders(page.value);
    }
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to delete this work order."),
      "Delete failed"
    );
  } finally {
    deletingWorkOrderId.value = null;
  }
}

function canDeleteWorkOrder(workOrder) {
  return auth.isManager && getWorkOrderState(workOrder) !== WORK_ORDER_STATES.inProgress;
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadWorkOrders(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadWorkOrders(page.value + 1);
}

onMounted(() => {
  loadFilterReferences();
  loadWorkOrders();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Operations"
      title="Work orders"
      subtitle="Track scheduled jobs, technician assignments, bays, and status flow."
      :icon="ClipboardList"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh work orders"
          @click="loadWorkOrders()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton v-if="auth.isManager" :to="{ name: 'work-order-create' }">
          <Plus :size="18" />
          <span>New work order</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Work order summary">
      <SummaryCard label="Total matching" :value="totalItems" />
      <SummaryCard label="Scheduled on page" :value="scheduledCount" />
      <SummaryCard label="In progress on page" :value="inProgressCount" />
    </SummaryGrid>

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasWorkOrders"
      :error-message="loadErrorMessage"
      loading-message="Loading work orders..."
      empty-title="No work orders found"
      empty-message="Create a work order or adjust filters to find workshop jobs."
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadWorkOrders()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="filters.searchTerm"
          search-placeholder="Search by vehicle, technician, task, or work order ID"
          :disabled="isLoading"
          :result-label="resultLabel"
          @submit="applyFilters"
          @clear="applyFilters"
        >
          <template #filters>
            <select v-model="filters.state" class="filter-control" :disabled="isLoading">
              <option value="">All states</option>
              <option
                v-for="stateOption in WORK_ORDER_STATE_OPTIONS"
                :key="stateOption.value"
                :value="stateOption.value"
              >
                {{ stateOption.label }}
              </option>
            </select>

            <select v-model="filters.bay" class="filter-control" :disabled="isLoading">
              <option value="">All bays</option>
              <option
                v-for="bayOption in WORK_ORDER_BAY_OPTIONS"
                :key="bayOption.value"
                :value="bayOption.value"
              >
                {{ bayOption.label }}
              </option>
            </select>

            <select
              v-if="auth.isManager"
              v-model="filters.laborId"
              class="filter-control"
              :disabled="isLoading"
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

            <select
              v-if="auth.isManager"
              v-model="filters.vehicleId"
              class="filter-control filter-control--wide"
              :disabled="isLoading"
            >
              <option value="">All vehicles</option>
              <option v-for="vehicle in vehicleOptions" :key="vehicle.id" :value="vehicle.id">
                {{ vehicle.label }}
              </option>
            </select>

            <input
              v-model="filters.startDateFrom"
              class="filter-control"
              type="date"
              :disabled="isLoading"
              aria-label="Start date from"
            />

            <input
              v-model="filters.startDateTo"
              class="filter-control"
              type="date"
              :disabled="isLoading"
              aria-label="Start date to"
            />

            <select v-model="filters.sort" class="filter-control" :disabled="isLoading">
              <option
                v-for="sortOption in WORK_ORDER_SORT_OPTIONS"
                :key="sortOption.value"
                :value="sortOption.value"
              >
                {{ sortOption.label }}
              </option>
            </select>
          </template>

          <template #actions>
            <ActionButton variant="secondary" type="submit" :disabled="isLoading">
              <Search :size="17" />
              <span>Apply</span>
            </ActionButton>

            <ActionButton
              v-if="hasActiveFilters"
              variant="ghost"
              type="button"
              :disabled="isLoading"
              @click="resetFilters"
            >
              <X :size="17" />
              <span>Reset</span>
            </ActionButton>
          </template>
        </EntityFilterBar>
      </template>

      <template #emptyIcon>
        <ClipboardList :size="28" />
      </template>

      <template #emptyAction>
        <ActionButton v-if="auth.isManager && !hasActiveFilters" :to="{ name: 'work-order-create' }">
          <Plus :size="18" />
          <span>Create work order</span>
        </ActionButton>
        <ActionButton v-else-if="hasActiveFilters" variant="secondary" @click="resetFilters">
          <X :size="17" />
          <span>Reset filters</span>
        </ActionButton>
      </template>

      <div class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Work order</th>
              <th>Status</th>
              <th>Schedule</th>
              <th>Customer / Vehicle</th>
              <th>Technician</th>
              <th>Bay</th>
              <th>Tasks</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="workOrder in workOrders" :key="getWorkOrderId(workOrder)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar entity-avatar--work-order">
                    <ClipboardList :size="17" />
                  </span>
                  <div>
                    <strong>{{ formatWorkOrderCode(getWorkOrderId(workOrder)) }}</strong>
                    <small>{{ getWorkOrderId(workOrder) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <StatusChip
                  :label="getStateLabel(getWorkOrderState(workOrder))"
                  :tone="getStateTone(getWorkOrderState(workOrder))"
                />
              </td>
              <td>
                <div class="info-stack">
                  <span>
                    <CalendarDays :size="14" />
                    {{ formatWorkOrderWindow(workOrder) }}
                  </span>
                </div>
              </td>
              <td>
                <div class="info-stack">
                  <strong>{{ getCustomerName(getCustomer(workOrder)) }}</strong>
                  <span>
                    <Car :size="14" />
                    {{ getVehicleLabel(getVehicle(workOrder)) }}
                  </span>
                  <small v-if="getVehiclePlate(getVehicle(workOrder))">
                    {{ getVehiclePlate(getVehicle(workOrder)) }}
                  </small>
                </div>
              </td>
              <td>
                <div class="info-stack">
                  <span>
                    <UserCog :size="14" />
                    {{ getEmployeeName(getEmployee(workOrder)) }}
                  </span>
                </div>
              </td>
              <td>
                <StatusChip :label="getBayLabel(readValue(workOrder, 'bay', 'Bay'))" tone="neutral" />
              </td>
              <td>
                <StatusChip
                  :label="`${getWorkOrderRepairTaskLines(workOrder).length} tasks`"
                  tone="service"
                  :icon="Wrench"
                />
                <div v-if="readValue(workOrder, 'totalTime', 'TotalTime', 0)" class="task-time">
                  {{ formatMinutes(readValue(workOrder, "totalTime", "TotalTime", 0)) }}
                </div>
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'work-order-details', params: { id: getWorkOrderId(workOrder) } }"
                    aria-label="Open work order"
                  >
                    <Eye :size="16" />
                  </ActionButton>
                  <ActionButton
                    v-if="canDeleteWorkOrder(workOrder)"
                    variant="danger"
                    size="sm"
                    icon-only
                    :disabled="deletingWorkOrderId === getWorkOrderId(workOrder)"
                    aria-label="Delete work order"
                    @click="handleDelete(workOrder)"
                  >
                    <Trash2 v-if="deletingWorkOrderId !== getWorkOrderId(workOrder)" :size="16" />
                    <RefreshCw v-else class="spinning" :size="16" />
                  </ActionButton>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </EntityListShell>
  </PageShell>
</template>

<style scoped>
.filter-control {
  min-height: 42px;
  min-width: 132px;
  padding: 0 10px;
  color: #111827;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  outline: none;
}

.filter-control--wide {
  min-width: 210px;
}

.entity-avatar--work-order {
  color: #0f766e;
  background: #ccfbf1;
}

.info-stack {
  display: grid;
  gap: 5px;
  min-width: 190px;
}

.info-stack strong {
  color: #111827;
  font-size: 14px;
}

.info-stack span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

.info-stack small,
.task-time {
  color: #94a3b8;
  font-size: 12px;
  font-weight: 800;
}

.task-time {
  margin-top: 7px;
}
</style>
