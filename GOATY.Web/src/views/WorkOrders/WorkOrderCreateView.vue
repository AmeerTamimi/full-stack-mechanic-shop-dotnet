<script setup>
import { ArrowLeft, ClipboardList, LoaderCircle, Save, Wrench } from "@lucide/vue";
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ErrorState from "@/components/Shared/ErrorState.vue";
import FormField from "@/components/Shared/FormField.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import { getCustomers } from "@/services/customers.service";
import { getEmployees } from "@/services/employees.service";
import { getRepairTasks } from "@/services/repairTasks.service";
import { addWorkOrder } from "@/services/workOrders.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMinutes, formatMoney } from "@/utils/formatters";
import { asArray, readValue } from "@/utils/objectAccess";
import {
  formatWorkOrderCode,
  getCustomerName,
  getCustomerVehicles,
  getEmployeeName,
  getRepairTaskId,
  getRepairTaskName,
  getVehicleLabel,
  getVehiclePlate,
  WORK_ORDER_BAY_OPTIONS,
} from "@/utils/workOrders";

const router = useRouter();
const ui = useUiStore();

const customers = ref([]);
const employees = ref([]);
const repairTasks = ref([]);
const isLoading = ref(false);
const isSaving = ref(false);
const loadErrorMessage = ref("");

const form = reactive({
  customerId: "",
  vehicleId: "",
  employeeId: "",
  startTime: getDefaultStartTime(),
  bay: "",
  discount: "0",
  quantity: "1",
  repairTaskIds: [],
});

const errors = reactive({
  customerId: "",
  vehicleId: "",
  employeeId: "",
  startTime: "",
  bay: "",
  discount: "",
  quantity: "",
  repairTaskIds: "",
});

const selectedCustomer = computed(() => {
  return customers.value.find((customer) => readValue(customer, "id", "Id") === form.customerId);
});
const customerVehicleOptions = computed(() => getCustomerVehicles(selectedCustomer.value));
const technicianOptions = computed(() => {
  return employees.value.filter((employee) => Number(readValue(employee, "role", "Role")) === 2);
});
const selectedRepairTasks = computed(() => {
  return repairTasks.value.filter((repairTask) =>
    form.repairTaskIds.includes(getRepairTaskId(repairTask))
  );
});
const selectedTaskTotal = computed(() => {
  return selectedRepairTasks.value.reduce(
    (total, repairTask) => total + Number(readValue(repairTask, "costEstimated", "CostEstimated", 0)),
    0
  );
});
const selectedTaskTime = computed(() => {
  return selectedRepairTasks.value.reduce(
    (total, repairTask) => total + Number(readValue(repairTask, "timeEstimated", "TimeEstimated", 0)),
    0
  );
});
const canSubmit = computed(() => {
  return (
    !isLoading.value &&
    !isSaving.value &&
    form.customerId &&
    form.vehicleId &&
    form.employeeId &&
    form.startTime &&
    form.bay &&
    form.repairTaskIds.length > 0
  );
});

watch(
  () => form.customerId,
  () => {
    form.vehicleId = "";
  }
);

function getDefaultStartTime() {
  const date = new Date(Date.now() + 60 * 60 * 1000);
  const roundedMinutes = Math.ceil(date.getMinutes() / 15) * 15;

  date.setMinutes(roundedMinutes, 0, 0);

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function clearErrors() {
  errors.customerId = "";
  errors.vehicleId = "";
  errors.employeeId = "";
  errors.startTime = "";
  errors.bay = "";
  errors.discount = "";
  errors.quantity = "";
  errors.repairTaskIds = "";
}

function validateForm() {
  clearErrors();

  const discount = Number(form.discount);
  const quantity = Number(form.quantity);
  const startTime = new Date(form.startTime);

  if (!form.customerId) {
    errors.customerId = "Choose a customer.";
  }

  if (!form.vehicleId) {
    errors.vehicleId = "Choose a vehicle.";
  }

  if (!form.employeeId) {
    errors.employeeId = "Choose a technician.";
  }

  if (!form.startTime) {
    errors.startTime = "Choose a start time.";
  } else if (Number.isNaN(startTime.getTime())) {
    errors.startTime = "Choose a valid start time.";
  } else if (startTime.getTime() <= Date.now()) {
    errors.startTime = "Start time must be in the future.";
  }

  if (!form.bay) {
    errors.bay = "Choose a bay.";
  }

  if (form.discount === "" || Number.isNaN(discount)) {
    errors.discount = "Discount is required.";
  } else if (discount < 0 || discount > 100) {
    errors.discount = "Discount must be between 0 and 100.";
  }

  if (form.quantity === "" || Number.isNaN(quantity)) {
    errors.quantity = "Quantity is required.";
  } else if (!Number.isInteger(quantity) || quantity <= 0) {
    errors.quantity = "Quantity must be a positive whole number.";
  }

  if (!form.repairTaskIds.length) {
    errors.repairTaskIds = "Choose at least one repair task.";
  }

  return Object.values(errors).every((error) => !error);
}

async function loadOptions() {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const [customersResponse, employeesResponse, repairTasksResponse] = await Promise.all([
      getCustomers({ Page: 1, PageSize: 100 }),
      getEmployees({ Page: 1, PageSize: 100 }),
      getRepairTasks({ Page: 1, PageSize: 100 }),
    ]);

    customers.value = normalizePaginatedResponse(customersResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
    employees.value = normalizePaginatedResponse(employeesResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
    repairTasks.value = normalizePaginatedResponse(repairTasksResponse.data, {
      page: 1,
      pageSize: 100,
    }).items;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Unable to load work order form options."
    );
  } finally {
    isLoading.value = false;
  }
}

function buildPayload() {
  return {
    customerId: form.customerId,
    vehicleId: form.vehicleId,
    employeeId: form.employeeId,
    startTime: form.startTime,
    bay: Number(form.bay),
    discount: Number(form.discount),
    quantity: Number(form.quantity),
    workOrderRepairTasks: form.repairTaskIds.map((repairTaskId) => ({ repairTaskId })),
  };
}

async function handleSubmit() {
  if (!validateForm()) return;

  isSaving.value = true;

  try {
    const { data } = await addWorkOrder(buildPayload());
    const workOrderId = readValue(data, "id", "Id");

    ui.showSuccessToast(
      `${formatWorkOrderCode(workOrderId)} was scheduled.`,
      "Work order created"
    );

    if (workOrderId) {
      await router.push({ name: "work-order-details", params: { id: workOrderId } });
    } else {
      await router.push({ name: "work-orders" });
    }
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to create work order. Please try again."),
      "Create work order failed"
    );
  } finally {
    isSaving.value = false;
  }
}

function getRepairTaskParts(repairTask) {
  return asArray(readValue(repairTask, "parts", "Parts", []));
}

onMounted(() => {
  loadOptions();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Operations"
      title="Create work order"
      subtitle="Schedule a customer vehicle with a technician, bay, and repair task plan."
      :icon="ClipboardList"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'work-orders' }">
          <ArrowLeft :size="18" />
          <span>Back to work orders</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Work order details" subtitle="Choose the job owner, assigned technician, time, bay, and repair tasks.">
      <template #icon>
        <ClipboardList :size="26" />
      </template>

      <LoadingState v-if="isLoading" message="Loading work order options..." />

      <ErrorState
        v-else-if="loadErrorMessage"
        title="Unable to prepare form"
        :message="loadErrorMessage"
        @retry="loadOptions"
      />

      <form v-else class="crud-form" novalidate @submit.prevent="handleSubmit">
        <FormField id="work-order-customer" label="Customer" :error="errors.customerId">
          <select
            id="work-order-customer"
            v-model="form.customerId"
            :aria-invalid="Boolean(errors.customerId)"
            aria-describedby="work-order-customer-error"
            @blur="validateForm"
          >
            <option value="" disabled>Select customer</option>
            <option
              v-for="customer in customers"
              :key="readValue(customer, 'id', 'Id')"
              :value="readValue(customer, 'id', 'Id')"
            >
              {{ getCustomerName(customer) }}
            </option>
          </select>
        </FormField>

        <FormField id="work-order-vehicle" label="Vehicle" :error="errors.vehicleId">
          <select
            id="work-order-vehicle"
            v-model="form.vehicleId"
            :disabled="!form.customerId"
            :aria-invalid="Boolean(errors.vehicleId)"
            aria-describedby="work-order-vehicle-error"
            @blur="validateForm"
          >
            <option value="" disabled>
              {{ form.customerId ? "Select vehicle" : "Choose customer first" }}
            </option>
            <option
              v-for="vehicle in customerVehicleOptions"
              :key="readValue(vehicle, 'id', 'Id')"
              :value="readValue(vehicle, 'id', 'Id')"
            >
              {{ getVehicleLabel(vehicle) }} - {{ getVehiclePlate(vehicle) }}
            </option>
          </select>
        </FormField>

        <div class="field-grid">
          <FormField id="work-order-technician" label="Technician" :error="errors.employeeId">
            <select
              id="work-order-technician"
              v-model="form.employeeId"
              :aria-invalid="Boolean(errors.employeeId)"
              aria-describedby="work-order-technician-error"
              @blur="validateForm"
            >
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

          <FormField id="work-order-bay" label="Bay" :error="errors.bay">
            <select
              id="work-order-bay"
              v-model="form.bay"
              :aria-invalid="Boolean(errors.bay)"
              aria-describedby="work-order-bay-error"
              @blur="validateForm"
            >
              <option value="" disabled>Select bay</option>
              <option v-for="bay in WORK_ORDER_BAY_OPTIONS" :key="bay.value" :value="bay.value">
                {{ bay.label }}
              </option>
            </select>
          </FormField>
        </div>

        <div class="field-grid">
          <FormField id="work-order-start" label="Start time" :error="errors.startTime">
            <input
              id="work-order-start"
              v-model="form.startTime"
              type="datetime-local"
              :aria-invalid="Boolean(errors.startTime)"
              aria-describedby="work-order-start-error"
              @blur="validateForm"
            />
          </FormField>

          <FormField id="work-order-discount" label="Discount percent" :error="errors.discount">
            <input
              id="work-order-discount"
              v-model="form.discount"
              type="number"
              min="0"
              max="100"
              step="0.01"
              :aria-invalid="Boolean(errors.discount)"
              aria-describedby="work-order-discount-error"
              @blur="validateForm"
            />
          </FormField>
        </div>

        <FormField id="work-order-quantity" label="Global task quantity" :error="errors.quantity">
          <input
            id="work-order-quantity"
            v-model="form.quantity"
            type="number"
            min="1"
            step="1"
            :aria-invalid="Boolean(errors.quantity)"
            aria-describedby="work-order-quantity-error"
            @blur="validateForm"
          />
        </FormField>

        <section class="task-picker">
          <div class="task-picker__header">
            <div>
              <h3>Repair tasks</h3>
              <p>Choose one or more task templates for this work order.</p>
            </div>
            <div class="task-picker__summary">
              <StatusChip :label="`${form.repairTaskIds.length} selected`" tone="service" :icon="Wrench" />
              <StatusChip :label="formatMinutes(selectedTaskTime)" tone="info" />
              <StatusChip :label="formatMoney(selectedTaskTotal)" tone="success" />
            </div>
          </div>

          <p v-if="errors.repairTaskIds" class="section-error">{{ errors.repairTaskIds }}</p>

          <div class="task-option-grid">
            <label
              v-for="repairTask in repairTasks"
              :key="getRepairTaskId(repairTask)"
              class="task-option"
            >
              <input
                v-model="form.repairTaskIds"
                type="checkbox"
                :value="getRepairTaskId(repairTask)"
                @change="validateForm"
              />
              <span class="task-option__content">
                <strong>{{ getRepairTaskName(repairTask) }}</strong>
                <small>{{ readValue(repairTask, "description", "Description") }}</small>
                <span class="task-option__meta">
                  <StatusChip
                    :label="formatMinutes(readValue(repairTask, 'timeEstimated', 'TimeEstimated', 0))"
                    tone="info"
                    size="sm"
                  />
                  <StatusChip
                    :label="formatMoney(readValue(repairTask, 'costEstimated', 'CostEstimated', 0))"
                    tone="success"
                    size="sm"
                  />
                  <StatusChip
                    :label="`${getRepairTaskParts(repairTask).length} parts`"
                    tone="neutral"
                    size="sm"
                  />
                </span>
              </span>
            </label>
          </div>
        </section>

        <ActionButton type="submit" size="lg" block :disabled="!canSubmit">
          <LoaderCircle v-if="isSaving" class="spinning" :size="18" />
          <Save v-else :size="18" />
          <span>{{ isSaving ? "Creating work order..." : "Create work order" }}</span>
        </ActionButton>
      </form>
    </FormPanel>
  </PageShell>
</template>

<style scoped>
.task-picker {
  display: grid;
  gap: 14px;
}

.task-picker__header {
  display: flex;
  justify-content: space-between;
  gap: 18px;
}

.task-picker__header h3 {
  margin: 0;
  color: #111827;
  font-size: 18px;
}

.task-picker__header p {
  margin: 5px 0 0;
  color: #6b7280;
  font-size: 13px;
  font-weight: 700;
}

.task-picker__summary {
  display: flex;
  align-items: flex-start;
  justify-content: flex-end;
  gap: 8px;
  flex-wrap: wrap;
}

.section-error {
  margin: 0;
  color: #be123c;
  font-size: 13px;
  font-weight: 700;
}

.task-option-grid {
  display: grid;
  gap: 12px;
}

.task-option {
  display: grid;
  grid-template-columns: 18px minmax(0, 1fr);
  gap: 12px;
  align-items: flex-start;
  padding: 14px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
}

.task-option input {
  margin-top: 3px;
  accent-color: #f59e0b;
}

.task-option__content {
  display: grid;
  gap: 7px;
}

.task-option__content strong {
  color: #111827;
  font-size: 15px;
}

.task-option__content small {
  color: #64748b;
  font-size: 13px;
  line-height: 1.45;
}

.task-option__meta {
  display: flex;
  gap: 7px;
  flex-wrap: wrap;
}

@media (max-width: 720px) {
  .task-picker__header {
    align-items: stretch;
    flex-direction: column;
  }

  .task-picker__summary {
    justify-content: flex-start;
  }
}
</style>
