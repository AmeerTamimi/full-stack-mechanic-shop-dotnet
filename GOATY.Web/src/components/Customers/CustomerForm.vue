<script setup>
import { Car, LoaderCircle, Plus, Save, Trash2 } from "@lucide/vue";
import { computed, reactive, watch } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";

const props = defineProps({
  initialCustomer: {
    type: Object,
    default: null,
  },
  isSubmitting: {
    type: Boolean,
    default: false,
  },
  submitLabel: {
    type: String,
    default: "Save customer",
  },
  submittingLabel: {
    type: String,
    default: "Saving customer...",
  },
});

const emit = defineEmits(["submit"]);

const currentYear = new Date().getFullYear();
const phonePattern = /^(?:0|970|972)5(6|9)\d{7}$/;
const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const form = reactive({
  firstName: "",
  lastName: "",
  phone: "",
  email: "",
  address: "",
  vehicles: [],
});

const errors = reactive({
  firstName: "",
  lastName: "",
  phone: "",
  email: "",
  address: "",
  vehicles: "",
  vehicleRows: [],
});

const canSubmit = computed(() => {
  return (
    !props.isSubmitting &&
    form.firstName.trim() &&
    form.lastName.trim() &&
    form.phone.trim() &&
    form.email.trim() &&
    form.address.trim() &&
    form.vehicles.length > 0
  );
});

function getValue(source, camelKey, pascalKey, fallback = "") {
  return source?.[camelKey] ?? source?.[pascalKey] ?? fallback;
}

function createLocalId() {
  return crypto.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(16).slice(2)}`;
}

function createVehicle(vehicle = {}) {
  return {
    id: getValue(vehicle, "id", "Id", createLocalId()),
    localId: createLocalId(),
    brand: getValue(vehicle, "brand", "Brand"),
    model: getValue(vehicle, "model", "Model"),
    year: String(getValue(vehicle, "year", "Year", currentYear)),
    licensePlate: getValue(vehicle, "licensePlate", "LicensePlate"),
  };
}

function syncForm(customer) {
  form.firstName = getValue(customer, "firstName", "FirstName");
  form.lastName = getValue(customer, "lastName", "LastName");
  form.phone = getValue(customer, "phone", "Phone");
  form.email = getValue(customer, "email", "Email");
  form.address = getValue(customer, "address", "Address");

  const vehicles = getValue(customer, "vehicles", "Vehicles", []);
  form.vehicles = vehicles.length ? vehicles.map(createVehicle) : [createVehicle()];
  clearErrors();
}

watch(
  () => props.initialCustomer,
  (customer) => {
    syncForm(customer);
  },
  { immediate: true }
);

function clearErrors() {
  errors.firstName = "";
  errors.lastName = "";
  errors.phone = "";
  errors.email = "";
  errors.address = "";
  errors.vehicles = "";
  errors.vehicleRows = form.vehicles.map(() => ({
    brand: "",
    model: "",
    year: "",
    licensePlate: "",
  }));
}

function validateName(value, label) {
  const trimmedValue = value.trim();

  if (trimmedValue.length < 5) {
    return `${label} must be at least 5 characters.`;
  }

  if (trimmedValue.length > 50) {
    return `${label} must be 50 characters or less.`;
  }

  return "";
}

function validateVehicle(vehicle, index, normalizedPlates) {
  const rowErrors = errors.vehicleRows[index];
  const year = Number(vehicle.year);
  const plate = vehicle.licensePlate.trim();
  const normalizedPlate = plate.toUpperCase();

  if (!vehicle.brand.trim()) {
    rowErrors.brand = "Brand is required.";
  } else if (vehicle.brand.trim().length > 50) {
    rowErrors.brand = "Brand must be 50 characters or less.";
  }

  if (!vehicle.model.trim()) {
    rowErrors.model = "Model is required.";
  } else if (vehicle.model.trim().length > 50) {
    rowErrors.model = "Model must be 50 characters or less.";
  }

  if (!vehicle.year || Number.isNaN(year) || !Number.isInteger(year)) {
    rowErrors.year = "Year is required.";
  } else if (year < 1900 || year > currentYear) {
    rowErrors.year = `Year must be between 1900 and ${currentYear}.`;
  }

  if (!plate) {
    rowErrors.licensePlate = "License plate is required.";
  } else if (plate.length > 20) {
    rowErrors.licensePlate = "License plate must be 20 characters or less.";
  } else if (normalizedPlates.filter((value) => value === normalizedPlate).length > 1) {
    rowErrors.licensePlate = "License plate is duplicated.";
  }
}

function validateForm() {
  clearErrors();

  errors.firstName = validateName(form.firstName, "First name");
  errors.lastName = validateName(form.lastName, "Last name");

  if (!phonePattern.test(form.phone.trim())) {
    errors.phone = "Use a valid Palestinian mobile number, like 0591234567.";
  }

  if (!emailPattern.test(form.email.trim())) {
    errors.email = "Enter a valid email address.";
  } else if (form.email.trim().length > 256) {
    errors.email = "Email must be 256 characters or less.";
  }

  if (!form.address.trim()) {
    errors.address = "Address is required.";
  } else if (form.address.trim().length > 300) {
    errors.address = "Address must be 300 characters or less.";
  }

  if (!form.vehicles.length) {
    errors.vehicles = "At least one vehicle is required.";
  }

  const normalizedPlates = form.vehicles.map((vehicle) =>
    vehicle.licensePlate.trim().toUpperCase()
  );

  form.vehicles.forEach((vehicle, index) => {
    validateVehicle(vehicle, index, normalizedPlates);
  });

  return (
    !errors.firstName &&
    !errors.lastName &&
    !errors.phone &&
    !errors.email &&
    !errors.address &&
    !errors.vehicles &&
    errors.vehicleRows.every(
      (row) => !row.brand && !row.model && !row.year && !row.licensePlate
    )
  );
}

function addVehicle() {
  form.vehicles.push(createVehicle());
  errors.vehicleRows.push({
    brand: "",
    model: "",
    year: "",
    licensePlate: "",
  });
}

function removeVehicle(index) {
  if (form.vehicles.length <= 1) return;

  form.vehicles.splice(index, 1);
  errors.vehicleRows.splice(index, 1);
}

function buildPayload() {
  return {
    firstName: form.firstName.trim(),
    lastName: form.lastName.trim(),
    phone: form.phone.trim(),
    email: form.email.trim(),
    address: form.address.trim(),
    vehicles: form.vehicles.map((vehicle) => ({
      id: vehicle.id,
      brand: vehicle.brand.trim(),
      model: vehicle.model.trim(),
      year: Number(vehicle.year),
      licensePlate: vehicle.licensePlate.trim().toUpperCase(),
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
    <div class="field-grid">
      <FormField id="customer-first-name" label="First name" :error="errors.firstName">
        <input
          id="customer-first-name"
          v-model="form.firstName"
          type="text"
          maxlength="50"
          placeholder="Mohammad"
          :aria-invalid="Boolean(errors.firstName)"
          aria-describedby="customer-first-name-error"
          @blur="validateForm"
        />
      </FormField>

      <FormField id="customer-last-name" label="Last name" :error="errors.lastName">
        <input
          id="customer-last-name"
          v-model="form.lastName"
          type="text"
          maxlength="50"
          placeholder="Hassan"
          :aria-invalid="Boolean(errors.lastName)"
          aria-describedby="customer-last-name-error"
          @blur="validateForm"
        />
      </FormField>
    </div>

    <div class="field-grid">
      <FormField id="customer-phone" label="Phone" :error="errors.phone">
        <input
          id="customer-phone"
          v-model="form.phone"
          type="tel"
          placeholder="0591234567"
          :aria-invalid="Boolean(errors.phone)"
          aria-describedby="customer-phone-error"
          @blur="validateForm"
        />
      </FormField>

      <FormField id="customer-email" label="Email" :error="errors.email">
        <input
          id="customer-email"
          v-model="form.email"
          type="email"
          maxlength="256"
          placeholder="customer@example.com"
          :aria-invalid="Boolean(errors.email)"
          aria-describedby="customer-email-error"
          @blur="validateForm"
        />
      </FormField>
    </div>

    <FormField id="customer-address" label="Address" :error="errors.address">
      <input
        id="customer-address"
        v-model="form.address"
        type="text"
        maxlength="300"
        placeholder="Ramallah, Palestine"
        :aria-invalid="Boolean(errors.address)"
        aria-describedby="customer-address-error"
        @blur="validateForm"
      />
    </FormField>

    <section class="vehicles-section">
      <div class="vehicles-header">
        <div>
          <h3>Vehicles</h3>
          <p>At least one vehicle is required for a customer.</p>
        </div>
        <ActionButton
          variant="secondary"
          type="button"
          :disabled="isSubmitting"
          @click="addVehicle"
        >
          <Plus :size="17" />
          <span>Add vehicle</span>
        </ActionButton>
      </div>

      <p v-if="errors.vehicles" class="section-error">{{ errors.vehicles }}</p>

      <article
        v-for="(vehicle, index) in form.vehicles"
        :key="vehicle.localId"
        class="vehicle-card"
      >
        <div class="vehicle-card__header">
          <div class="vehicle-card__title">
            <span>
              <Car :size="18" />
            </span>
            <strong>Vehicle {{ index + 1 }}</strong>
          </div>

          <ActionButton
            variant="danger"
            size="sm"
            icon-only
            type="button"
            :disabled="isSubmitting || form.vehicles.length <= 1"
            aria-label="Remove vehicle"
            @click="removeVehicle(index)"
          >
            <Trash2 :size="16" />
          </ActionButton>
        </div>

        <div class="field-grid">
          <FormField
            :id="`vehicle-${index}-brand`"
            label="Brand"
            :error="errors.vehicleRows[index]?.brand"
          >
            <input
              :id="`vehicle-${index}-brand`"
              v-model="vehicle.brand"
              type="text"
              maxlength="50"
              placeholder="Toyota"
              :aria-invalid="Boolean(errors.vehicleRows[index]?.brand)"
              :aria-describedby="`vehicle-${index}-brand-error`"
              @blur="validateForm"
            />
          </FormField>

          <FormField
            :id="`vehicle-${index}-model`"
            label="Model"
            :error="errors.vehicleRows[index]?.model"
          >
            <input
              :id="`vehicle-${index}-model`"
              v-model="vehicle.model"
              type="text"
              maxlength="50"
              placeholder="Corolla"
              :aria-invalid="Boolean(errors.vehicleRows[index]?.model)"
              :aria-describedby="`vehicle-${index}-model-error`"
              @blur="validateForm"
            />
          </FormField>
        </div>

        <div class="field-grid">
          <FormField
            :id="`vehicle-${index}-year`"
            label="Year"
            :error="errors.vehicleRows[index]?.year"
          >
            <input
              :id="`vehicle-${index}-year`"
              v-model="vehicle.year"
              type="number"
              min="1900"
              :max="currentYear"
              step="1"
              placeholder="2020"
              :aria-invalid="Boolean(errors.vehicleRows[index]?.year)"
              :aria-describedby="`vehicle-${index}-year-error`"
              @blur="validateForm"
            />
          </FormField>

          <FormField
            :id="`vehicle-${index}-plate`"
            label="License plate"
            :error="errors.vehicleRows[index]?.licensePlate"
          >
            <input
              :id="`vehicle-${index}-plate`"
              v-model="vehicle.licensePlate"
              type="text"
              maxlength="20"
              placeholder="123-456"
              :aria-invalid="Boolean(errors.vehicleRows[index]?.licensePlate)"
              :aria-describedby="`vehicle-${index}-plate-error`"
              @blur="validateForm"
            />
          </FormField>
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
.vehicles-section {
  display: grid;
  gap: 14px;
  margin-top: 4px;
}

.vehicles-header,
.vehicle-card__header,
.vehicle-card__title {
  display: flex;
  align-items: center;
}

.vehicles-header {
  justify-content: space-between;
  gap: 16px;
}

.vehicles-header h3 {
  margin: 0;
  color: #111827;
  font-size: 18px;
}

.vehicles-header p {
  margin: 5px 0 0;
  color: #6b7280;
  font-size: 13px;
  font-weight: 700;
}

.section-error {
  margin: 0;
  color: #be123c;
  font-size: 13px;
  font-weight: 700;
}

.vehicle-card {
  display: grid;
  gap: 16px;
  padding: 16px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.vehicle-card__header {
  justify-content: space-between;
  gap: 12px;
}

.vehicle-card__title {
  gap: 9px;
}

.vehicle-card__title span {
  display: grid;
  place-items: center;
  width: 34px;
  height: 34px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.13);
  border-radius: 8px;
}

.vehicle-card__title strong {
  color: #111827;
  font-size: 15px;
}

@media (max-width: 720px) {
  .vehicles-header,
  .vehicle-card__header {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
