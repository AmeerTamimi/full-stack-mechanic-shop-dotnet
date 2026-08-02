<script setup>
import {
  ArrowLeft,
  CalendarDays,
  Car,
  ClipboardList,
  DollarSign,
  LoaderCircle,
  Package,
  RefreshCw,
  Save,
  Trash2,
  UserCog,
  Wrench,
} from "@lucide/vue";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import ErrorState from "@/components/Shared/ErrorState.vue";
import FormField from "@/components/Shared/FormField.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { getEmployees } from "@/services/employees.service";
import { getRepairTasks } from "@/services/repairTasks.service";
import {
  assignWorkOrderTechnician,
  deleteWorkOrder,
  getWorkOrder,
  relocateWorkOrder,
  updateWorkOrderRepairTasks,
  updateWorkOrderState,
  updateWorkOrderVehicle,
} from "@/services/workOrders.service";
import { useAuthStore } from "@/store/modules/auth";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatDateTime, formatMinutes, formatMoney } from "@/utils/formatters";
import { asArray, readValue } from "@/utils/objectAccess";
import {
  formatWorkOrderCode,
  formatWorkOrderTotals,
  formatWorkOrderWindow,
  getAllowedNextStates,
  getBayLabel,
  getCustomer,
  getCustomerName,
  getCustomerVehicles,
  getEmployee,
  getEmployeeName,
  getRepairTaskFromLine,
  getRepairTaskId,
  getRepairTaskName,
  getStateLabel,
  getStateTone,
  getVehicle,
  getVehicleLabel,
  getVehiclePlate,
  getWorkOrderId,
  getWorkOrderRepairTaskLines,
  getWorkOrderState,
  getWorkOrderTaskCost,
  getWorkOrderTaskName,
  getWorkOrderTaskTime,
  isScheduled,
  toDateTimeLocalValue,
  WORK_ORDER_BAY_OPTIONS,
  WORK_ORDER_STATES,
} from "@/utils/workOrders";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();
const ui = useUiStore();

const workOrder = ref(null);
const employees = ref([]);
const repairTasks = ref([]);
const isLoading = ref(false);
const isReferencesLoading = ref(false);
const loadErrorMessage = ref("");
const savingAction = ref("");

const managerForm = reactive({
  employeeId: "",
  startTime: "",
  bay: "",
  vehicleId: "",
  repairTaskIds: [],
});

const workOrderId = computed(() => route.params.id?.toString());
const state = computed(() => getWorkOrderState(workOrder.value));
const stateMeta = computed(() => ({
  label: getStateLabel(state.value),
  tone: getStateTone(state.value),
}));
const totals = computed(() => formatWorkOrderTotals(workOrder.value));
const customer = computed(() => getCustomer(workOrder.value));
const vehicle = computed(() => getVehicle(workOrder.value));
const employee = computed(() => getEmployee(workOrder.value));
const invoice = computed(() => readValue(workOrder.value, "invoice", "Invoice", null));
const repairTaskLines = computed(() => getWorkOrderRepairTaskLines(workOrder.value));
const allowedNextStates = computed(() => getAllowedNextStates(state.value));
const canManageScheduledFields = computed(() => auth.isManager && isScheduled(workOrder.value));
const canDelete = computed(() => auth.isManager && state.value !== WORK_ORDER_STATES.inProgress);
const customerVehicleOptions = computed(() => getCustomerVehicles(customer.value));
const technicianOptions = computed(() => {
  return employees.value.filter((item) => Number(readValue(item, "role", "Role")) === 2);
});
const selectedReplacementTasks = computed(() => {
  return repairTasks.value.filter((repairTask) =>
    managerForm.repairTaskIds.includes(getRepairTaskId(repairTask))
  );
});
const replacementTotal = computed(() => {
  return selectedReplacementTasks.value.reduce(
    (total, repairTask) => total + Number(readValue(repairTask, "costEstimated", "CostEstimated", 0)),
    0
  );
});

function fillManagerForm() {
  if (!workOrder.value) return;

  managerForm.employeeId = readValue(employee.value, "id", "Id");
  managerForm.startTime = toDateTimeLocalValue(readValue(workOrder.value, "startTime", "StartTime"));
  managerForm.bay = String(readValue(workOrder.value, "bay", "Bay"));
  managerForm.vehicleId = readValue(vehicle.value, "id", "Id");
  managerForm.repairTaskIds = repairTaskLines.value
    .map((line) => getRepairTaskId(getRepairTaskFromLine(line)))
    .filter(Boolean);
}

async function loadWorkOrder() {
  if (!workOrderId.value) {
    loadErrorMessage.value = "Missing work order id.";
    return;
  }

  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getWorkOrder(workOrderId.value);
    workOrder.value = data;
    fillManagerForm();
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Unable to load this work order."
    );
  } finally {
    isLoading.value = false;
  }
}

async function loadManagerReferences() {
  if (!auth.isManager) return;

  isReferencesLoading.value = true;

  try {
    const [employeesResponse, repairTasksResponse] = await Promise.all([
      getEmployees({ Page: 1, PageSize: 100 }),
      getRepairTasks({ Page: 1, PageSize: 100 }),
    ]);

    employees.value = normalizePaginatedResponse(employeesResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
    repairTasks.value = normalizePaginatedResponse(repairTasksResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to load management options."),
      "Management options unavailable"
    );
  } finally {
    isReferencesLoading.value = false;
  }
}

async function refreshAfterAction(message, title) {
  ui.showSuccessToast(message, title);
  await loadWorkOrder();
}

async function handleStateUpdate(nextState) {
  const nextStateLabel = getStateLabel(nextState);

  if (Number(nextState) === WORK_ORDER_STATES.cancelled) {
    const shouldCancel = await ui.confirm({
      title: "Cancel work order?",
      message: `${formatWorkOrderCode(workOrderId.value)} will move to Cancelled.`,
      confirmText: "Cancel work order",
      cancelText: "Keep current status",
      variant: "danger",
    });

    if (!shouldCancel) return;
  }

  savingAction.value = `state-${nextState}`;

  try {
    await updateWorkOrderState(workOrderId.value, Number(nextState));
    await refreshAfterAction(
      `${formatWorkOrderCode(workOrderId.value)} moved to ${nextStateLabel}.`,
      "Status updated"
    );
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update work order status."),
      "Status update failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleAssignTechnician() {
  if (!managerForm.employeeId) return;

  savingAction.value = "technician";

  try {
    await assignWorkOrderTechnician(workOrderId.value, managerForm.employeeId);
    await refreshAfterAction("Technician assignment was updated.", "Technician updated");
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to assign technician."),
      "Technician update failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleRelocate() {
  if (!managerForm.startTime || !managerForm.bay) return;

  savingAction.value = "relocation";

  try {
    await relocateWorkOrder(workOrderId.value, {
      startTime: managerForm.startTime,
      bay: Number(managerForm.bay),
    });
    await refreshAfterAction("Schedule and bay were updated.", "Work order relocated");
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to relocate work order."),
      "Relocation failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleUpdateVehicle() {
  if (!managerForm.vehicleId) return;

  savingAction.value = "vehicle";

  try {
    await updateWorkOrderVehicle(workOrderId.value, managerForm.vehicleId);
    await refreshAfterAction("Vehicle assignment was updated.", "Vehicle updated");
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update vehicle."),
      "Vehicle update failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleUpdateRepairTasks() {
  if (!managerForm.repairTaskIds.length) return;

  savingAction.value = "repair-tasks";

  try {
    await updateWorkOrderRepairTasks(workOrderId.value, managerForm.repairTaskIds);
    await refreshAfterAction("Repair task plan was replaced.", "Repair tasks updated");
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update repair tasks."),
      "Repair task update failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleDelete() {
  const shouldDelete = await ui.confirm({
    title: "Delete work order?",
    message: `This will permanently remove ${formatWorkOrderCode(workOrderId.value)}.`,
    confirmText: "Delete work order",
    cancelText: "Keep work order",
    variant: "danger",
  });

  if (!shouldDelete) return;

  savingAction.value = "delete";

  try {
    await deleteWorkOrder(workOrderId.value);
    ui.showSuccessToast(`${formatWorkOrderCode(workOrderId.value)} was deleted.`, "Work order deleted");
    await router.push({ name: "work-orders" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to delete this work order."),
      "Delete failed"
    );
  } finally {
    savingAction.value = "";
  }
}

function getInvoiceStatusLabel(status) {
  const value = Number(status);

  if (value === 1) return "Paid";
  if (value === 2) return "Not paid";
  if (value === 3) return "Refunded";

  return "Unknown";
}

function getInvoiceStatusTone(status) {
  const value = Number(status);

  if (value === 1) return "success";
  if (value === 2) return "warning";
  if (value === 3) return "danger";

  return "neutral";
}

function getRepairTaskParts(repairTask) {
  return asArray(readValue(repairTask, "parts", "Parts", []));
}

onMounted(async () => {
  await loadWorkOrder();
  await loadManagerReferences();
  fillManagerForm();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Operations"
      :title="workOrder ? formatWorkOrderCode(getWorkOrderId(workOrder)) : 'Work order details'"
      subtitle="Review assignment, schedule, status, costs, and repair task plan."
      :icon="ClipboardList"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'work-orders' }">
          <ArrowLeft :size="18" />
          <span>Back to work orders</span>
        </ActionButton>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh work order"
          @click="loadWorkOrder"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>
      </template>
    </PageHeader>

    <LoadingState v-if="isLoading" message="Loading work order..." />

    <ErrorState
      v-else-if="loadErrorMessage"
      title="Unable to load work order"
      :message="loadErrorMessage"
      @retry="loadWorkOrder"
    />

    <template v-else-if="workOrder">
      <SummaryGrid aria-label="Work order summary">
        <SummaryCard label="Status" :value="stateMeta.label" />
        <SummaryCard label="Bay" :value="getBayLabel(readValue(workOrder, 'bay', 'Bay'))" />
        <SummaryCard label="Total cost" :value="totals.totalCost" />
      </SummaryGrid>

      <section class="details-grid">
        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Assignment</span>
              <h2>Customer and technician</h2>
            </div>
            <StatusChip :label="stateMeta.label" :tone="stateMeta.tone" />
          </div>

          <div class="detail-list">
            <div class="detail-row">
              <span>
                <Car :size="16" />
                Customer
              </span>
              <strong>{{ getCustomerName(customer) }}</strong>
            </div>
            <div class="detail-row">
              <span>
                <Car :size="16" />
                Vehicle
              </span>
              <strong>{{ getVehicleLabel(vehicle) }}</strong>
              <small v-if="getVehiclePlate(vehicle)">{{ getVehiclePlate(vehicle) }}</small>
            </div>
            <div class="detail-row">
              <span>
                <UserCog :size="16" />
                Technician
              </span>
              <strong>{{ getEmployeeName(employee) }}</strong>
            </div>
          </div>
        </ContentPanel>

        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Schedule</span>
              <h2>Time and bay</h2>
            </div>
            <StatusChip :label="getBayLabel(readValue(workOrder, 'bay', 'Bay'))" tone="neutral" />
          </div>

          <div class="detail-list">
            <div class="detail-row">
              <span>
                <CalendarDays :size="16" />
                Window
              </span>
              <strong>{{ formatWorkOrderWindow(workOrder) }}</strong>
            </div>
            <div class="detail-row">
              <span>
                <Wrench :size="16" />
                Total time
              </span>
              <strong>{{ totals.totalTime }}</strong>
            </div>
          </div>
        </ContentPanel>
      </section>

      <section class="details-grid">
        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Money lane</span>
              <h2>Cost summary</h2>
            </div>
            <StatusChip :label="`${readValue(workOrder, 'discount', 'Discount', 0)}% discount`" tone="warning" />
          </div>

          <div class="detail-list">
            <div class="detail-row">
              <span>
                <DollarSign :size="16" />
                Total cost
              </span>
              <strong>{{ totals.totalCost }}</strong>
            </div>
            <div class="detail-row">
              <span>
                <Package :size="16" />
                Parts cost
              </span>
              <strong>{{ totals.partsCost }}</strong>
            </div>
            <div class="detail-row">
              <span>
                <Wrench :size="16" />
                Labor cost
              </span>
              <strong>{{ totals.laborCost }}</strong>
            </div>
          </div>
        </ContentPanel>

        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Billing</span>
              <h2>Invoice</h2>
            </div>
            <StatusChip
              v-if="invoice"
              :label="getInvoiceStatusLabel(readValue(invoice, 'status', 'Status'))"
              :tone="getInvoiceStatusTone(readValue(invoice, 'status', 'Status'))"
            />
            <StatusChip v-else label="No invoice" tone="neutral" />
          </div>

          <div v-if="invoice" class="detail-list">
            <div class="detail-row">
              <span>Issued</span>
              <strong>{{ formatDateTime(readValue(invoice, "issuedAt", "IssuedAt")) }}</strong>
            </div>
            <div class="detail-row">
              <span>Total</span>
              <strong>{{ formatMoney(readValue(invoice, "total", "Total", 0)) }}</strong>
            </div>
          </div>

          <p v-else class="panel-note">An invoice will appear here once billing is generated.</p>
        </ContentPanel>
      </section>

      <ContentPanel class="details-panel task-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">Repair plan</span>
            <h2>Repair tasks</h2>
          </div>
          <StatusChip :label="`${repairTaskLines.length} tasks`" tone="service" :icon="Wrench" />
        </div>

        <div class="task-list">
          <article v-for="line in repairTaskLines" :key="getRepairTaskId(getRepairTaskFromLine(line))" class="task-row">
            <div>
              <strong>{{ getWorkOrderTaskName(line) }}</strong>
              <small>{{ formatMinutes(getWorkOrderTaskTime(line)) }}</small>
            </div>
            <StatusChip :label="formatMoney(getWorkOrderTaskCost(line))" tone="success" />
          </article>
        </div>
      </ContentPanel>

      <ContentPanel class="details-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">State machine</span>
            <h2>Status actions</h2>
          </div>
          <StatusChip :label="stateMeta.label" :tone="stateMeta.tone" />
        </div>

        <div v-if="allowedNextStates.length" class="action-strip">
          <ActionButton
            v-for="nextState in allowedNextStates"
            :key="nextState"
            :variant="Number(nextState) === WORK_ORDER_STATES.cancelled ? 'danger' : 'primary'"
            :disabled="Boolean(savingAction)"
            @click="handleStateUpdate(nextState)"
          >
            <LoaderCircle
              v-if="savingAction === `state-${nextState}`"
              class="spinning"
              :size="18"
            />
            <Save v-else :size="18" />
            <span>Move to {{ getStateLabel(nextState) }}</span>
          </ActionButton>
        </div>
        <p v-else class="panel-note">This work order has no available status transitions.</p>
      </ContentPanel>

      <ContentPanel v-if="auth.isManager" class="details-panel manager-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">Manager controls</span>
            <h2>Manage scheduled work</h2>
          </div>
          <StatusChip
            :label="canManageScheduledFields ? 'Editable' : 'Locked'"
            :tone="canManageScheduledFields ? 'success' : 'neutral'"
          />
        </div>

        <LoadingState v-if="isReferencesLoading" message="Loading management options..." />

        <p v-else-if="!canManageScheduledFields" class="panel-note">
          Assignment, vehicle, schedule, and repair-task edits are available only while the work order is Scheduled.
        </p>

        <div v-else class="manager-grid">
          <form class="manager-card" @submit.prevent="handleAssignTechnician">
            <FormField id="manage-technician" label="Technician">
              <select id="manage-technician" v-model="managerForm.employeeId">
                <option value="" disabled>Select technician</option>
                <option
                  v-for="technician in technicianOptions"
                  :key="readValue(technician, 'id', 'Id')"
                  :value="readValue(technician, 'id', 'Id')"
                >
                  {{ getEmployeeName(technician) }}
                </option>
              </select>
            </FormField>
            <ActionButton type="submit" :disabled="savingAction === 'technician' || !managerForm.employeeId">
              <LoaderCircle v-if="savingAction === 'technician'" class="spinning" :size="18" />
              <Save v-else :size="18" />
              <span>Assign technician</span>
            </ActionButton>
          </form>

          <form class="manager-card" @submit.prevent="handleRelocate">
            <div class="field-grid">
              <FormField id="manage-start-time" label="Start time">
                <input id="manage-start-time" v-model="managerForm.startTime" type="datetime-local" />
              </FormField>
              <FormField id="manage-bay" label="Bay">
                <select id="manage-bay" v-model="managerForm.bay">
                  <option value="" disabled>Select bay</option>
                  <option v-for="bay in WORK_ORDER_BAY_OPTIONS" :key="bay.value" :value="bay.value">
                    {{ bay.label }}
                  </option>
                </select>
              </FormField>
            </div>
            <ActionButton type="submit" :disabled="savingAction === 'relocation' || !managerForm.startTime || !managerForm.bay">
              <LoaderCircle v-if="savingAction === 'relocation'" class="spinning" :size="18" />
              <Save v-else :size="18" />
              <span>Relocate</span>
            </ActionButton>
          </form>

          <form class="manager-card" @submit.prevent="handleUpdateVehicle">
            <FormField id="manage-vehicle" label="Vehicle">
              <select id="manage-vehicle" v-model="managerForm.vehicleId">
                <option value="" disabled>Select vehicle</option>
                <option
                  v-for="vehicleOption in customerVehicleOptions"
                  :key="readValue(vehicleOption, 'id', 'Id')"
                  :value="readValue(vehicleOption, 'id', 'Id')"
                >
                  {{ getVehicleLabel(vehicleOption) }} - {{ getVehiclePlate(vehicleOption) }}
                </option>
              </select>
            </FormField>
            <ActionButton type="submit" :disabled="savingAction === 'vehicle' || !managerForm.vehicleId">
              <LoaderCircle v-if="savingAction === 'vehicle'" class="spinning" :size="18" />
              <Save v-else :size="18" />
              <span>Update vehicle</span>
            </ActionButton>
          </form>

          <form class="manager-card manager-card--wide" @submit.prevent="handleUpdateRepairTasks">
            <div class="manager-card__heading">
              <strong>Repair task plan</strong>
              <span>{{ formatMoney(replacementTotal) }}</span>
            </div>
            <div class="replacement-task-grid">
              <label
                v-for="repairTask in repairTasks"
                :key="getRepairTaskId(repairTask)"
                class="replacement-task"
              >
                <input
                  v-model="managerForm.repairTaskIds"
                  type="checkbox"
                  :value="getRepairTaskId(repairTask)"
                />
                <span>
                  <strong>{{ getRepairTaskName(repairTask) }}</strong>
                  <small>
                    {{ formatMinutes(readValue(repairTask, "timeEstimated", "TimeEstimated", 0)) }}
                    /
                    {{ formatMoney(readValue(repairTask, "costEstimated", "CostEstimated", 0)) }}
                    /
                    {{ getRepairTaskParts(repairTask).length }} parts
                  </small>
                </span>
              </label>
            </div>
            <ActionButton type="submit" :disabled="savingAction === 'repair-tasks' || !managerForm.repairTaskIds.length">
              <LoaderCircle v-if="savingAction === 'repair-tasks'" class="spinning" :size="18" />
              <Save v-else :size="18" />
              <span>Replace repair tasks</span>
            </ActionButton>
          </form>
        </div>

        <div v-if="canDelete" class="delete-zone">
          <div>
            <strong>Delete work order</strong>
            <span>Remove this work order permanently.</span>
          </div>
          <ActionButton variant="danger" :disabled="savingAction === 'delete'" @click="handleDelete">
            <LoaderCircle v-if="savingAction === 'delete'" class="spinning" :size="18" />
            <Trash2 v-else :size="18" />
            <span>Delete</span>
          </ActionButton>
        </div>
      </ContentPanel>
    </template>
  </PageShell>
</template>

<style scoped>
.details-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.details-panel {
  padding: 22px;
  margin-bottom: 16px;
}

.panel-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
}

.panel-kicker {
  display: block;
  margin-bottom: 6px;
  color: #0f766e;
  font-size: 11px;
  font-weight: 900;
  text-transform: uppercase;
}

.panel-heading h2 {
  margin: 0;
  color: #111827;
  font-size: 21px;
}

.detail-list {
  display: grid;
  gap: 12px;
}

.detail-row {
  display: grid;
  gap: 5px;
  padding: 12px;
  background: #f8fafc;
  border: 1px solid #eef2f7;
  border-radius: 8px;
}

.detail-row span {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.detail-row strong {
  color: #111827;
  font-size: 15px;
}

.detail-row small,
.panel-note {
  color: #64748b;
  font-size: 13px;
  line-height: 1.5;
}

.panel-note {
  margin: 0;
}

.task-list {
  display: grid;
  gap: 10px;
}

.task-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 12px;
  background: #f8fafc;
  border: 1px solid #eef2f7;
  border-radius: 8px;
}

.task-row strong,
.task-row small {
  display: block;
}

.task-row small {
  margin-top: 4px;
  color: #64748b;
}

.action-strip {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.manager-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.manager-card {
  display: grid;
  gap: 14px;
  padding: 16px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.manager-card--wide {
  grid-column: 1 / -1;
}

.manager-card__heading,
.delete-zone {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
}

.manager-card__heading strong,
.delete-zone strong {
  color: #111827;
}

.manager-card__heading span,
.delete-zone span {
  color: #64748b;
  font-size: 13px;
  font-weight: 800;
}

.replacement-task-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 10px;
}

.replacement-task {
  display: grid;
  grid-template-columns: 18px minmax(0, 1fr);
  gap: 10px;
  padding: 12px;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
}

.replacement-task input {
  margin-top: 3px;
  accent-color: #f59e0b;
}

.replacement-task strong,
.replacement-task small {
  display: block;
}

.replacement-task small {
  margin-top: 4px;
  color: #64748b;
  font-size: 12px;
  line-height: 1.45;
}

.delete-zone {
  margin-top: 18px;
  padding: 16px;
  background: #fff1f2;
  border: 1px solid #fecdd3;
  border-radius: 8px;
}

@media (max-width: 900px) {
  .details-grid,
  .manager-grid,
  .replacement-task-grid {
    grid-template-columns: 1fr;
  }

  .manager-card--wide {
    grid-column: auto;
  }
}

@media (max-width: 720px) {
  .panel-heading,
  .task-row,
  .manager-card__heading,
  .delete-zone {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
