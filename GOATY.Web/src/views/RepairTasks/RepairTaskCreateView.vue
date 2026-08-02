<script setup>
import { ArrowLeft, Wrench } from "@lucide/vue";
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import RepairTaskForm from "@/components/RepairTasks/RepairTaskForm.vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getParts } from "@/services/parts.service";
import { addRepairTask } from "@/services/repairTasks.service";
import { useUiStore } from "@/store/modules/ui";

const router = useRouter();
const ui = useUiStore();
const parts = ref([]);
const isPartsLoading = ref(false);
const isSaving = ref(false);

function getBackendErrorMessage(error, fallbackMessage) {
  const data = error.response?.data;

  if (!data) {
    return fallbackMessage;
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

  return fallbackMessage;
}

function formatMoney(value) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(Number(value || 0));
}

async function loadParts() {
  isPartsLoading.value = true;

  try {
    const { data } = await getParts({
      Page: 1,
      PageSize: 100,
    });

    parts.value = data.items ?? data.Items ?? [];
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load inventory parts. Create repair tasks after inventory is available."
      ),
      "Unable to load parts"
    );
  } finally {
    isPartsLoading.value = false;
  }
}

async function handleSubmit(payload) {
  isSaving.value = true;

  try {
    await addRepairTask(payload);

    ui.showSuccessToast(
      `Repair task "${payload.name}" created for ${formatMoney(payload.costEstimated)}.`,
      "Repair task created"
    );
    await router.push({ name: "repair-tasks" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to create repair task. Please try again."),
      "Create repair task failed"
    );
  } finally {
    isSaving.value = false;
  }
}

onMounted(() => {
  loadParts();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Service catalog"
      title="Create repair task"
      subtitle="Create a reusable repair template with time, labor, and required parts."
      :icon="Wrench"
      tone="service"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'repair-tasks' }">
          <ArrowLeft :size="18" />
          <span>Back to tasks</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel
      title="Repair task details"
      subtitle="Name, description, duration, costs, and at least one part are required."
    >
      <template #icon>
        <Wrench :size="26" />
      </template>

      <RepairTaskForm
        :parts-options="parts"
        :is-parts-loading="isPartsLoading"
        :is-submitting="isSaving"
        submit-label="Create repair task"
        submitting-label="Creating repair task..."
        @submit="handleSubmit"
      />
    </FormPanel>
  </PageShell>
</template>
