<script setup>
import { ArrowLeft, LoaderCircle, PackagePlus, Save } from "@lucide/vue";
import { computed, reactive, ref } from "vue";
import { useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
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
  <PageShell size="form">
    <PageHeader
      eyebrow="Inventory"
      title="Create part"
      subtitle="Add a new part to the workshop inventory."
      :icon="PackagePlus"
      tone="inventory"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'parts' }">
          <ArrowLeft :size="18" />
          <span>Back to parts</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Part details" subtitle="Name, cost, and stock quantity are required.">
      <template #icon>
        <PackagePlus :size="26" />
      </template>

      <form class="crud-form" novalidate @submit.prevent="handleSubmit">
        <FormField id="part-name" label="Part name" :error="errors.name">
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
        </FormField>

        <div class="field-grid">
          <FormField id="part-cost" label="Cost" :error="errors.cost">
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
          </FormField>

          <FormField id="part-quantity" label="Quantity" :error="errors.quantity">
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
          </FormField>
        </div>

        <ActionButton type="submit" size="lg" block :disabled="!canSubmit">
          <LoaderCircle v-if="isLoading" class="spinning" :size="18" />
          <Save v-else :size="18" />
          <span>{{ isLoading ? "Creating part..." : "Create part" }}</span>
        </ActionButton>
      </form>
    </FormPanel>
  </PageShell>
</template>
