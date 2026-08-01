<script setup>
import { ArrowLeft, LoaderCircle, PackagePlus, Save } from "@lucide/vue";
import { computed, reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import { addPart } from "@/services/parts.service";
import { useUiStore } from "@/store/modules/ui";

const router = useRouter();
const ui = useUiStore();

const form = reactive({
  name: "",
  cost: "",
  quantity: "",
});

const errors = reactive({
  name: "",
  cost: "",
  quantity: "",
});

const isLoading = ref(false);

const canSubmit = computed(() => {
  return !isLoading.value && form.name.trim() && form.cost !== "" && form.quantity !== "";
});

function clearFieldErrors() {
  errors.name = "";
  errors.cost = "";
  errors.quantity = "";
}

function validateForm() {
  clearFieldErrors();

  const trimmedName = form.name.trim();
  const cost = Number(form.cost);
  const quantity = Number(form.quantity);

  if (trimmedName.length < 3) {
    errors.name = "Part name must be at least 3 characters.";
  } else if (trimmedName.length > 50) {
    errors.name = "Part name must be 50 characters or less.";
  }

  if (form.cost === "" || Number.isNaN(cost)) {
    errors.cost = "Cost is required.";
  } else if (cost <= 10) {
    errors.cost = "Cost must be more than 10.";
  }

  if (form.quantity === "" || Number.isNaN(quantity)) {
    errors.quantity = "Quantity is required.";
  } else if (!Number.isInteger(quantity)) {
    errors.quantity = "Quantity must be a whole number.";
  } else if (quantity < 0) {
    errors.quantity = "Quantity must be greater than or equal to 0.";
  }

  return !errors.name && !errors.cost && !errors.quantity;
}

function formatMoney(value) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(value);
}

function getBackendErrorMessage(error) {
  const data = error.response?.data;

  if (!data) {
    return "Unable to create part. Please check your connection and try again.";
  }

  if (typeof data === "string") {
    return data;
  }

  if (data.detail) {
    return data.detail;
  }

  if (data.title) {
    return data.title;
  }

  if (data.errors) {
    const messages = Object.values(data.errors).flat();
    if (messages.length) {
      return messages.join(" ");
    }
  }

  return "Unable to create part. Please try again.";
}

async function handleSubmit() {
  if (!validateForm()) return;

  isLoading.value = true;

  try {
    const payload = {
      name: form.name.trim(),
      cost: Number(form.cost),
      quantity: Number(form.quantity),
    };

    await addPart(payload);

    ui.showSuccessToast(
      `Part "${payload.name}" created. Cost: ${formatMoney(payload.cost)}, Quantity: ${payload.quantity}.`,
      "Part created"
    );
    await router.push({ name: "parts" });
  } catch (error) {
    ui.showErrorToast(getBackendErrorMessage(error), "Create part failed");
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <main class="part-create-page">
    <section class="part-create-shell">
      <header class="page-header">
        <div>
          <p class="page-eyebrow">Inventory</p>
          <h1>Create part</h1>
          <p class="page-subtitle">Add a new part to the workshop inventory.</p>
        </div>

        <RouterLink class="back-link" :to="{ name: 'parts' }">
          <ArrowLeft :size="18" />
          <span>Back to parts</span>
        </RouterLink>
      </header>

      <section class="form-panel">
        <div class="form-intro">
          <div class="intro-icon" aria-hidden="true">
            <PackagePlus :size="26" />
          </div>
          <div>
            <h2>Part details</h2>
            <p>Name, cost, and stock quantity are required.</p>
          </div>
        </div>

        <form class="part-form" novalidate @submit.prevent="handleSubmit">
          <div class="field">
            <label for="part-name">Part name</label>
            <input
              id="part-name"
              v-model="form.name"
              type="text"
              maxlength="50"
              placeholder="Brake pads"
              :aria-invalid="Boolean(errors.name)"
              aria-describedby="part-name-error"
              @blur="validateForm"
            />
            <p v-if="errors.name" id="part-name-error" class="field-error">
              {{ errors.name }}
            </p>
          </div>

          <div class="field-grid">
            <div class="field">
              <label for="part-cost">Cost</label>
              <input
                id="part-cost"
                v-model="form.cost"
                type="number"
                min="0"
                step="0.01"
                placeholder="45.00"
                :aria-invalid="Boolean(errors.cost)"
                aria-describedby="part-cost-error"
                @blur="validateForm"
              />
              <p v-if="errors.cost" id="part-cost-error" class="field-error">
                {{ errors.cost }}
              </p>
            </div>

            <div class="field">
              <label for="part-quantity">Quantity</label>
              <input
                id="part-quantity"
                v-model="form.quantity"
                type="number"
                min="0"
                step="1"
                placeholder="20"
                :aria-invalid="Boolean(errors.quantity)"
                aria-describedby="part-quantity-error"
                @blur="validateForm"
              />
              <p v-if="errors.quantity" id="part-quantity-error" class="field-error">
                {{ errors.quantity }}
              </p>
            </div>
          </div>

          <button class="submit-button" type="submit" :disabled="!canSubmit">
            <LoaderCircle v-if="isLoading" class="spinning" :size="18" />
            <Save v-else :size="18" />
            <span>{{ isLoading ? "Creating part..." : "Create part" }}</span>
          </button>
        </form>
      </section>
    </section>
  </main>
</template>

<style scoped>
.part-create-page {
  min-height: 100vh;
  padding: 32px;
  color: #111827;
  background:
    radial-gradient(circle at top left, rgba(245, 158, 11, 0.13), transparent 28%),
    linear-gradient(135deg, #f8fafc 0%, #edf2f7 100%);
}

.part-create-shell {
  width: min(860px, 100%);
  margin: 0 auto;
}

.page-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 24px;
  margin-bottom: 22px;
}

.page-eyebrow {
  margin: 0 0 8px;
  color: #b45309;
  font-size: 12px;
  font-weight: 900;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.page-header h1 {
  margin: 0;
  color: #111827;
  font-size: 42px;
  line-height: 1;
  letter-spacing: 0;
}

.page-subtitle {
  margin: 10px 0 0;
  color: #6b7280;
  font-size: 15px;
}

.back-link,
.submit-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 0;
  border-radius: 8px;
  font: inherit;
  font-weight: 800;
  text-decoration: none;
  cursor: pointer;
  transition:
    transform 160ms ease,
    box-shadow 160ms ease,
    background 160ms ease,
    color 160ms ease,
    opacity 160ms ease;
}

.back-link {
  min-height: 44px;
  gap: 8px;
  padding: 0 14px;
  color: #374151;
  background: #fff;
  border: 1px solid #e5e7eb;
}

.back-link:hover {
  transform: translateY(-1px);
  color: #111827;
  box-shadow: 0 14px 26px rgba(15, 23, 42, 0.08);
}

.form-panel {
  padding: 28px;
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 20px 48px rgba(15, 23, 42, 0.1);
}

.form-intro {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 24px;
  padding-bottom: 22px;
  border-bottom: 1px solid #eef2f7;
}

.intro-icon {
  display: grid;
  place-items: center;
  width: 54px;
  height: 54px;
  flex: 0 0 54px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.13);
  border-radius: 8px;
}

.form-intro h2 {
  margin: 0;
  color: #111827;
  font-size: 24px;
  letter-spacing: 0;
}

.form-intro p {
  margin: 6px 0 0;
  color: #6b7280;
  font-size: 14px;
}

.part-form {
  display: grid;
  gap: 18px;
}

.field-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.field {
  display: grid;
  gap: 8px;
}

.field label {
  color: #374151;
  font-size: 13px;
  font-weight: 900;
}

.field input {
  width: 100%;
  min-height: 50px;
  padding: 0 14px;
  color: #111827;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  outline: none;
  transition:
    border-color 160ms ease,
    box-shadow 160ms ease;
}

.field input:focus {
  border-color: rgba(245, 158, 11, 0.82);
  box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.16);
}

.field input[aria-invalid="true"] {
  border-color: #f43f5e;
}

.field input[aria-invalid="true"]:focus {
  box-shadow: 0 0 0 4px rgba(244, 63, 94, 0.14);
}

.field-error {
  margin: 0;
  color: #be123c;
  font-size: 13px;
  font-weight: 700;
}

.status-message {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 13px 14px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 750;
}

.status-message--error {
  color: #991b1b;
  background: #fff1f2;
  border: 1px solid #fecdd3;
}

.status-message--success {
  color: #166534;
  background: #f0fdf4;
  border: 1px solid #bbf7d0;
}

.submit-button {
  min-height: 52px;
  gap: 9px;
  margin-top: 4px;
  color: #fff;
  background: linear-gradient(135deg, #111827, #263142);
  box-shadow: 0 16px 30px rgba(15, 23, 42, 0.16);
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 22px 34px rgba(15, 23, 42, 0.21);
}

.submit-button:disabled {
  cursor: not-allowed;
  opacity: 0.62;
}

.spinning {
  animation: spin 850ms linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 720px) {
  .part-create-page {
    padding: 18px;
  }

  .page-header,
  .form-intro {
    align-items: flex-start;
    flex-direction: column;
  }

  .back-link {
    width: 100%;
  }

  .field-grid {
    grid-template-columns: 1fr;
  }
}
</style>
