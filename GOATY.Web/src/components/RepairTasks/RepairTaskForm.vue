<script setup>
import { computed, reactive, watch } from "vue";
import { LoaderCircle, Package, Plus, Save, Trash2, Wrench } from "@lucide/vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";

const props = defineProps({
  initialRepairTask: {
    type: Object,
    default: null,
  },
  partsOptions: {
    type: Array,
    default: () => [],
  },
  isPartsLoading: {
    type: Boolean,
    default: false,
  },
  isSubmitting: {
    type: Boolean,
    default: false,
  },
  submitLabel: {
    type: String,
    default: "Save repair task",
  },
  submittingLabel: {
    type: String,
    default: "Saving repair task...",
  },
});

const emit = defineEmits(["submit"]);

const timeOptions = [
  { value: "10", label: "10 minutes" },
  { value: "15", label: "15 minutes" },
  { value: "30", label: "30 minutes" },
  { value: "45", label: "45 minutes" },
  { value: "60", label: "1 hour" },
  { value: "90", label: "1 hour 30 minutes" },
  { value: "120", label: "2 hours" },
  { value: "150", label: "2 hours 30 minutes" },
  { value: "180", label: "3 hours" },
  { value: "360", label: "6 hours" },
];

const form = reactive({
  name: "",
  description: "",
  timeEstimated: "",
  costEstimated: "",
  technicianCost: "",
  parts: [],
});

const errors = reactive({
  name: "",
  description: "",
  timeEstimated: "",
  costEstimated: "",
  technicianCost: "",
  parts: "",
  partRows: [],
});

const normalizedParts = computed(() => {
  return props.partsOptions.map((part) => ({
    id: getValue(part, "id", "Id"),
    name: getValue(part, "name", "Name", "Unnamed part"),
    cost: Number(getValue(part, "cost", "Cost", 0)),
    quantity: Number(getValue(part, "quantity", "Quantity", 0)),
  }));
});

const partLookup = computed(() => {
  return normalizedParts.value.reduce((lookup, part) => {
    lookup[part.id] = part;
    return lookup;
  }, {});
});

const partsTotal = computed(() => {
  return form.parts.reduce((total, line) => {
    const part = getSelectedPart(line.partId);
    return total + part.cost * Number(line.quantity || 0);
  }, 0);
});

const estimatedMargin = computed(() => {
  return Number(form.costEstimated || 0) - partsTotal.value - Number(form.technicianCost || 0);
});

const canSubmit = computed(() => {
  return (
    !props.isSubmitting &&
    !props.isPartsLoading &&
    normalizedParts.value.length > 0 &&
    form.name.trim() &&
    form.description.trim() &&
    form.timeEstimated &&
    form.costEstimated !== "" &&
    form.technicianCost !== "" &&
    form.parts.length > 0 &&
    form.parts.every((line) => line.partId && line.quantity !== "")
  );
});

function getValue(source, camelKey, pascalKey, fallback = "") {
  return source?.[camelKey] ?? source?.[pascalKey] ?? fallback;
}

function createLocalId() {
  return crypto.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(16).slice(2)}`;
}

function createPartLine(line = {}) {
  const part = getValue(line, "part", "Part", null);

  return {
    localId: createLocalId(),
    partId: getValue(line, "partId", "PartId", getValue(part, "id", "Id")),
    quantity: String(getValue(line, "quantity", "Quantity", 1)),
  };
}

function syncForm(repairTask) {
  form.name = getValue(repairTask, "name", "Name");
  form.description = getValue(repairTask, "description", "Description");
  form.timeEstimated = String(getValue(repairTask, "timeEstimated", "TimeEstimated"));
  form.costEstimated = String(getValue(repairTask, "costEstimated", "CostEstimated"));
  form.technicianCost = String(getValue(repairTask, "technicianCost", "TechnicianCost"));

  const parts = getValue(repairTask, "parts", "Parts", []);
  form.parts = parts.length ? parts.map(createPartLine) : [createPartLine()];
  clearErrors();
}

watch(
  () => props.initialRepairTask,
  (repairTask) => {
    syncForm(repairTask);
  },
  { immediate: true }
);

function clearErrors() {
  errors.name = "";
  errors.description = "";
  errors.timeEstimated = "";
  errors.costEstimated = "";
  errors.technicianCost = "";
  errors.parts = "";
  errors.partRows = form.parts.map(() => ({
    partId: "",
    quantity: "",
  }));
}

function getSelectedPart(partId) {
  return partLookup.value[partId] ?? {
    id: "",
    name: "No part selected",
    cost: 0,
    quantity: 0,
  };
}

function formatMoney(value) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(Number(value || 0));
}

function validateForm() {
  clearErrors();

  const name = form.name.trim();
  const description = form.description.trim();
  const costEstimated = Number(form.costEstimated);
  const technicianCost = Number(form.technicianCost);
  const selectedPartIds = form.parts.map((line) => line.partId).filter(Boolean);

  if (name.length < 3) {
    errors.name = "Repair task name must be at least 3 characters.";
  } else if (name.length > 100) {
    errors.name = "Repair task name must be 100 characters or less.";
  }

  if (description.length < 10) {
    errors.description = "Description must be at least 10 characters.";
  } else if (description.length > 1000) {
    errors.description = "Description must be 1000 characters or less.";
  }

  if (!timeOptions.some((option) => option.value === form.timeEstimated)) {
    errors.timeEstimated = "Choose a valid estimated duration.";
  }

  if (form.costEstimated === "" || Number.isNaN(costEstimated)) {
    errors.costEstimated = "Estimated cost is required.";
  } else if (costEstimated < 50) {
    errors.costEstimated = "Estimated cost must be 50 or more.";
  } else if (costEstimated < partsTotal.value) {
    errors.costEstimated = `Estimated cost cannot be less than required parts total (${formatMoney(partsTotal.value)}).`;
  }

  if (form.technicianCost === "" || Number.isNaN(technicianCost)) {
    errors.technicianCost = "Technician cost is required.";
  } else if (technicianCost < 50) {
    errors.technicianCost = "Technician cost must be 50 or more.";
  }

  if (!form.parts.length) {
    errors.parts = "At least one required part is needed.";
  }

  form.parts.forEach((line, index) => {
    const rowErrors = errors.partRows[index];
    const quantity = Number(line.quantity);
    const isDuplicate = selectedPartIds.filter((partId) => partId === line.partId).length > 1;

    if (!line.partId) {
      rowErrors.partId = "Choose a part.";
    } else if (normalizedParts.value.length && !partLookup.value[line.partId]) {
      rowErrors.partId = "Choose a valid part.";
    } else if (isDuplicate) {
      rowErrors.partId = "This part is already added.";
    }

    if (line.quantity === "" || Number.isNaN(quantity)) {
      rowErrors.quantity = "Quantity is required.";
    } else if (!Number.isInteger(quantity)) {
      rowErrors.quantity = "Quantity must be a whole number.";
    } else if (quantity <= 0) {
      rowErrors.quantity = "Quantity must be greater than 0.";
    }
  });

  return (
    !errors.name &&
    !errors.description &&
    !errors.timeEstimated &&
    !errors.costEstimated &&
    !errors.technicianCost &&
    !errors.parts &&
    errors.partRows.every((row) => !row.partId && !row.quantity)
  );
}

function addPartLine() {
  form.parts.push(createPartLine());
  errors.partRows.push({
    partId: "",
    quantity: "",
  });
}

function removePartLine(index) {
  if (form.parts.length <= 1) return;

  form.parts.splice(index, 1);
  errors.partRows.splice(index, 1);
}

function buildPayload() {
  return {
    name: form.name.trim(),
    description: form.description.trim(),
    timeEstimated: Number(form.timeEstimated),
    costEstimated: Number(form.costEstimated),
    technicianCost: Number(form.technicianCost),
    parts: form.parts.map((line) => ({
      partId: line.partId,
      quantity: Number(line.quantity),
    })),
  };
}

function handleSubmit() {
  if (!validateForm()) return;

  emit("submit", buildPayload());
}
</script>

<template>
  <form class="crud-form" novalidate @submit.prevent="handleSubmit">
    <FormField id="repair-task-name" label="Task name" :error="errors.name">
      <input
        id="repair-task-name"
        v-model="form.name"
        type="text"
        maxlength="100"
        placeholder="Brake inspection"
        :aria-invalid="Boolean(errors.name)"
        aria-describedby="repair-task-name-error"
        @blur="validateForm"
      />
    </FormField>

    <FormField
      id="repair-task-description"
      label="Description"
      :error="errors.description"
    >
      <textarea
        id="repair-task-description"
        v-model="form.description"
        maxlength="1000"
        placeholder="Inspect pads, rotors, and brake fluid before estimating the work."
        :aria-invalid="Boolean(errors.description)"
        aria-describedby="repair-task-description-error"
        @blur="validateForm"
      ></textarea>
    </FormField>

    <div class="field-grid">
      <FormField
        id="repair-task-time"
        label="Estimated time"
        :error="errors.timeEstimated"
      >
        <select
          id="repair-task-time"
          v-model="form.timeEstimated"
          :aria-invalid="Boolean(errors.timeEstimated)"
          aria-describedby="repair-task-time-error"
          @blur="validateForm"
        >
          <option value="" disabled>Select duration</option>
          <option v-for="option in timeOptions" :key="option.value" :value="option.value">
            {{ option.label }}
          </option>
        </select>
      </FormField>

      <FormField
        id="repair-task-technician-cost"
        label="Technician cost"
        :error="errors.technicianCost"
      >
        <input
          id="repair-task-technician-cost"
          v-model="form.technicianCost"
          type="number"
          min="50"
          step="0.01"
          placeholder="75.00"
          :aria-invalid="Boolean(errors.technicianCost)"
          aria-describedby="repair-task-technician-cost-error"
          @blur="validateForm"
        />
      </FormField>
    </div>

    <FormField
      id="repair-task-cost"
      label="Customer estimated cost"
      :error="errors.costEstimated"
    >
      <input
        id="repair-task-cost"
        v-model="form.costEstimated"
        type="number"
        min="50"
        step="0.01"
        placeholder="180.00"
        :aria-invalid="Boolean(errors.costEstimated)"
        aria-describedby="repair-task-cost-error"
        @blur="validateForm"
      />
    </FormField>

    <section class="task-parts-section">
      <div class="task-parts-header">
        <div>
          <h3>Required parts</h3>
          <p>Pick inventory parts needed by this repair task template.</p>
        </div>

        <ActionButton
          type="button"
          variant="secondary"
          :disabled="isSubmitting || isPartsLoading || !normalizedParts.length"
          @click="addPartLine"
        >
          <Plus :size="17" />
          <span>Add part</span>
        </ActionButton>
      </div>

      <div class="cost-strip">
        <span>
          <Package :size="16" />
          Parts total
          <strong>{{ formatMoney(partsTotal) }}</strong>
        </span>
        <span :class="{ 'cost-strip__loss': estimatedMargin < 0 }">
          <Wrench :size="16" />
          Margin after labor
          <strong>{{ formatMoney(estimatedMargin) }}</strong>
        </span>
      </div>

      <p v-if="isPartsLoading" class="section-hint">Loading inventory parts...</p>
      <p v-else-if="!normalizedParts.length" class="section-error">
        Create at least one inventory part before adding repair tasks.
      </p>
      <p v-if="errors.parts" class="section-error">{{ errors.parts }}</p>

      <article
        v-for="(line, index) in form.parts"
        :key="line.localId"
        class="part-line-card"
      >
        <div class="part-line-card__header">
          <div class="part-line-card__title">
            <span>
              <Package :size="18" />
            </span>
            <strong>Part {{ index + 1 }}</strong>
          </div>

          <ActionButton
            type="button"
            variant="danger"
            size="sm"
            icon-only
            :disabled="isSubmitting || form.parts.length <= 1"
            aria-label="Remove required part"
            @click="removePartLine(index)"
          >
            <Trash2 :size="16" />
          </ActionButton>
        </div>

        <div class="field-grid">
          <FormField
            :id="`repair-task-part-${index}`"
            label="Part"
            :error="errors.partRows[index]?.partId"
          >
            <select
              :id="`repair-task-part-${index}`"
              v-model="line.partId"
              :disabled="isPartsLoading"
              :aria-invalid="Boolean(errors.partRows[index]?.partId)"
              :aria-describedby="`repair-task-part-${index}-error`"
              @blur="validateForm"
              @change="validateForm"
            >
              <option value="" disabled>Select part</option>
              <option v-for="part in normalizedParts" :key="part.id" :value="part.id">
                {{ part.name }} - {{ formatMoney(part.cost) }}
              </option>
            </select>
          </FormField>

          <FormField
            :id="`repair-task-part-quantity-${index}`"
            label="Quantity"
            :error="errors.partRows[index]?.quantity"
          >
            <input
              :id="`repair-task-part-quantity-${index}`"
              v-model="line.quantity"
              type="number"
              min="1"
              step="1"
              placeholder="1"
              :aria-invalid="Boolean(errors.partRows[index]?.quantity)"
              :aria-describedby="`repair-task-part-quantity-${index}-error`"
              @blur="validateForm"
            />
          </FormField>
        </div>

        <div class="line-total">
          <span>{{ getSelectedPart(line.partId).name }}</span>
          <strong>
            {{ formatMoney(getSelectedPart(line.partId).cost * Number(line.quantity || 0)) }}
          </strong>
        </div>
      </article>
    </section>

    <ActionButton type="submit" size="lg" block :disabled="!canSubmit">
      <LoaderCircle v-if="isSubmitting" class="spinning" :size="18" />
      <Save v-else :size="18" />
      <span>{{ isSubmitting ? submittingLabel : submitLabel }}</span>
    </ActionButton>
  </form>
</template>

<style scoped>
.task-parts-section {
  display: grid;
  gap: 14px;
}

.task-parts-header,
.part-line-card__header,
.part-line-card__title,
.cost-strip,
.cost-strip span,
.line-total {
  display: flex;
  align-items: center;
}

.task-parts-header,
.part-line-card__header,
.line-total {
  justify-content: space-between;
  gap: 16px;
}

.task-parts-header h3 {
  margin: 0;
  color: #111827;
  font-size: 18px;
}

.task-parts-header p,
.section-hint,
.section-error {
  margin: 5px 0 0;
  color: #6b7280;
  font-size: 13px;
  font-weight: 700;
}

.section-error {
  color: #be123c;
}

.cost-strip {
  flex-wrap: wrap;
  gap: 10px;
  padding: 12px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.cost-strip span {
  gap: 7px;
  min-height: 34px;
  padding: 0 10px;
  color: #475569;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
}

.cost-strip strong {
  color: #111827;
}

.cost-strip__loss strong {
  color: #b91c1c;
}

.part-line-card {
  display: grid;
  gap: 16px;
  padding: 16px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.part-line-card__title {
  gap: 9px;
}

.part-line-card__title span {
  display: grid;
  place-items: center;
  width: 34px;
  height: 34px;
  color: #b91c1c;
  background: rgba(239, 68, 68, 0.12);
  border-radius: 8px;
}

.part-line-card__title strong {
  color: #111827;
  font-size: 15px;
}

.line-total {
  padding: 12px 14px;
  color: #64748b;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 850;
}

.line-total strong {
  color: #111827;
  font-size: 15px;
}

@media (max-width: 720px) {
  .task-parts-header,
  .part-line-card__header,
  .line-total {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
