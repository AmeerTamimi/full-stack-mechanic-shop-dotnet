<script setup>
import { ArrowLeft, Pencil } from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import RepairTaskForm from "@/components/RepairTasks/RepairTaskForm.vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getParts } from "@/services/parts.service";
import { getRepairTask, updateRepairTask } from "@/services/repairTasks.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMoney } from "@/utils/formatters";

const route = useRoute();
const router = useRouter();
const ui = useUiStore();

const repairTaskId = computed(() => route.params.id?.toString());
const repairTask = ref(null);
const parts = ref([]);
const isLoading = ref(false);
const isPartsLoading = ref(false);
const isSaving = ref(false);

async function loadParts() {
  isPartsLoading.value = true;

  try {
    const { data } = await getParts({
      Page: 1,
      PageSize: 100,
    });

    parts.value = normalizePaginatedResponse(data, {
      page: 1,
      pageSize: 100,
    }).items;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load inventory parts. Editing is disabled until parts are available."
      ),
      "Unable to load parts"
    );
  } finally {
    isPartsLoading.value = false;
  }
}

async function loadRepairTask() {
  if (!repairTaskId.value) {
    ui.showErrorToast("Missing repair task id.", "Unable to load repair task");
    await router.push({ name: "repair-tasks" });
    return;
  }

  isLoading.value = true;

  try {
    const { data } = await getRepairTask(repairTaskId.value);
    repairTask.value = data;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load this repair task. It may have been deleted or you may not have access."
      ),
      "Unable to load repair task"
    );
    await router.push({ name: "repair-tasks" });
  } finally {
    isLoading.value = false;
  }
}

async function handleSubmit(payload) {
  isSaving.value = true;

  try {
    await updateRepairTask(repairTaskId.value, payload);

    ui.showSuccessToast(
      `Repair task "${payload.name}" updated for ${formatMoney(payload.costEstimated)}.`,
      "Repair task updated"
    );
    await router.push({ name: "repair-tasks" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update repair task. Please try again."),
      "Update repair task failed"
    );
  } finally {
    isSaving.value = false;
  }
}

onMounted(() => {
  loadParts();
  loadRepairTask();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Service catalog"
      title="Edit repair task"
      subtitle="Update the repair template, pricing, duration, and required parts."
      :icon="Pencil"
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
      subtitle="Changes will update the repair task template used by work orders."
    >
      <template #icon>
        <Pencil :size="25" />
      </template>

      <LoadingState v-if="isLoading" message="Loading repair task..." />

      <RepairTaskForm
        v-else
        :initial-repair-task="repairTask"
        :parts-options="parts"
        :is-parts-loading="isPartsLoading"
        :is-submitting="isSaving"
        submit-label="Save changes"
        submitting-label="Saving changes..."
        @submit="handleSubmit"
      />
    </FormPanel>
  </PageShell>
</template>
