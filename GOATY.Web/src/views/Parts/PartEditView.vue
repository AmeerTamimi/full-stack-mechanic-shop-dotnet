<script setup>
import { ArrowLeft, LoaderCircle, Pencil, Save } from "@lucide/vue";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getPart, updatePart } from "@/services/parts.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage } from "@/utils/api";
import { formatMoney } from "@/utils/formatters";
import { readValue } from "@/utils/objectAccess";

const route = useRoute();
const router = useRouter();
const ui = useUiStore();

const partId = computed(() => route.params.id?.toString());

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
const isSaving = ref(false);

const canSubmit = computed(() => {
  return (
    !isLoading.value &&
    !isSaving.value &&
    form.name.trim() &&
    form.cost !== "" &&
    form.quantity !== ""
  );
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

function fillForm(part) {
  form.name = readValue(part, "name", "Name");
  form.cost = String(readValue(part, "cost", "Cost"));
  form.quantity = String(readValue(part, "quantity", "Quantity"));
}

async function loadPart() {
  if (!partId.value) {
    ui.showErrorToast("Missing part id.", "Unable to load part");
    await router.push({ name: "parts" });
    return;
  }

  isLoading.value = true;

  try {
    const { data } = await getPart(partId.value);
    fillForm(data);
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load this part. It may have been deleted or you may not have access."
      ),
      "Unable to load part"
    );
    await router.push({ name: "parts" });
  } finally {
    isLoading.value = false;
  }
}

async function handleSubmit() {
  if (!validateForm()) return;

  isSaving.value = true;

  try {
    const payload = {
      name: form.name.trim(),
      cost: Number(form.cost),
      quantity: Number(form.quantity),
    };

    await updatePart(partId.value, payload);

    ui.showSuccessToast(
      `Part "${payload.name}" updated. Cost: ${formatMoney(payload.cost)}, Quantity: ${payload.quantity}.`,
      "Part updated"
    );
    await router.push({ name: "parts" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update part. Please try again."),
      "Update part failed"
    );
  } finally {
    isSaving.value = false;
  }
}

onMounted(() => {
  loadPart();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Inventory"
      title="Edit part"
      subtitle="Update part name, cost, and stock quantity."
      :icon="Pencil"
      tone="inventory"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'parts' }">
          <ArrowLeft :size="18" />
          <span>Back to parts</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Part details" subtitle="Changes will be saved to the inventory catalog.">
      <template #icon>
        <Pencil :size="25" />
      </template>

      <LoadingState v-if="isLoading" message="Loading part..." />

      <form v-else class="crud-form" novalidate @submit.prevent="handleSubmit">
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
          <LoaderCircle v-if="isSaving" class="spinning" :size="18" />
          <Save v-else :size="18" />
          <span>{{ isSaving ? "Saving changes..." : "Save changes" }}</span>
        </ActionButton>
      </form>
    </FormPanel>
  </PageShell>
</template>
