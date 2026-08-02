<script setup>
import {
  Clock3,
  DollarSign,
  Package,
  Pencil,
  Plus,
  RefreshCw,
  Trash2,
  Wrench,
} from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import EmptyState from "@/components/Shared/EmptyState.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import PaginationBar from "@/components/Shared/PaginationBar.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { deleteRepairTask, getRepairTasks } from "@/services/repairTasks.service";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();
const repairTasks = ref([]);
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingRepairTaskId = ref(null);

const hasRepairTasks = computed(() => repairTasks.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => {
  return Math.min(page.value * pageSize.value, totalItems.value);
});
const requiredPartsOnPage = computed(() => {
  return repairTasks.value.reduce((count, task) => count + getParts(task).length, 0);
});
const avgEstimatedCost = computed(() => {
  if (!repairTasks.value.length) return formatMoney(0);

  const total = repairTasks.value.reduce(
    (sum, task) => sum + Number(getValue(task, "costEstimated", "CostEstimated", 0)),
    0
  );

  return formatMoney(total / repairTasks.value.length);
});

const timeLabels = {
  10: "10 min",
  15: "15 min",
  30: "30 min",
  45: "45 min",
  60: "1 hr",
  90: "1 hr 30 min",
  120: "2 hr",
  150: "2 hr 30 min",
  180: "3 hr",
  360: "6 hr",
};

function getValue(source, camelKey, pascalKey, fallback = "") {
  return source?.[camelKey] ?? source?.[pascalKey] ?? fallback;
}

function getRepairTaskId(task) {
  return getValue(task, "id", "Id");
}

function getRepairTaskName(task) {
  return getValue(task, "name", "Name", "Unnamed repair task");
}

function getDescription(task) {
  return getValue(task, "description", "Description");
}

function getParts(task) {
  return getValue(task, "parts", "Parts", []);
}

function getPartName(line) {
  const part = getValue(line, "part", "Part", null);
  return getValue(part, "name", "Name", "Part");
}

function getLineQuantity(line) {
  return Number(getValue(line, "quantity", "Quantity", 0));
}

function getPartsTotal(task) {
  return getParts(task).reduce((total, line) => {
    const unitPrice = Number(getValue(line, "unitPrice", "UnitPrice", 0));
    return total + unitPrice * getLineQuantity(line);
  }, 0);
}

function formatMoney(value) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(Number(value || 0));
}

function formatTime(value) {
  return timeLabels[Number(value)] ?? `${value} min`;
}

function getErrorMessage(error) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.title ||
    "Something went wrong while loading repair tasks."
  );
}

async function loadRepairTasks(targetPage = page.value) {
  isLoading.value = true;

  try {
    const { data } = await getRepairTasks({
      Page: targetPage,
      PageSize: pageSize.value,
    });

    repairTasks.value = data.items ?? data.Items ?? [];
    page.value = data.page ?? data.Page ?? targetPage;
    pageSize.value = data.pageSize ?? data.PageSize ?? pageSize.value;
    totalItems.value = data.totalItems ?? data.TotalItems ?? repairTasks.value.length;
    totalPages.value = data.totalPages ?? data.TotalPages ?? 1;
  } catch (error) {
    ui.showErrorToast(getErrorMessage(error), "Unable to load repair tasks");
  } finally {
    isLoading.value = false;
  }
}

async function handleDelete(task) {
  const repairTaskId = getRepairTaskId(task);
  const repairTaskName = getRepairTaskName(task);

  if (!repairTaskId) return;

  const shouldDelete = await ui.confirm({
    title: "Delete repair task?",
    message: `This will permanently remove "${repairTaskName}" from the task templates.`,
    confirmText: "Delete task",
    cancelText: "Keep task",
    variant: "danger",
  });

  if (!shouldDelete) return;

  deletingRepairTaskId.value = repairTaskId;

  try {
    await deleteRepairTask(repairTaskId);

    ui.showSuccessToast(`"${repairTaskName}" was removed from repair tasks.`, "Task deleted");

    if (repairTasks.value.length === 1 && page.value > 1) {
      await loadRepairTasks(page.value - 1);
    } else {
      await loadRepairTasks(page.value);
    }
  } catch (error) {
    ui.showErrorToast(
      error.response?.data?.detail ||
        error.response?.data?.title ||
        "Unable to delete this repair task.",
      "Delete failed"
    );
  } finally {
    deletingRepairTaskId.value = null;
  }
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadRepairTasks(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadRepairTasks(page.value + 1);
}

onMounted(() => {
  loadRepairTasks();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Service catalog"
      title="Repair tasks"
      subtitle="Build reusable repair templates with labor estimates and required parts."
      :icon="Wrench"
      tone="service"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh repair tasks"
          @click="loadRepairTasks()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton :to="{ name: 'repair-task-create' }">
          <Plus :size="18" />
          <span>New task</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Repair tasks summary">
      <SummaryCard label="Total templates" :value="totalItems" />
      <SummaryCard label="Required parts on page" :value="requiredPartsOnPage" />
      <SummaryCard label="Average estimate" :value="avgEstimatedCost" />
    </SummaryGrid>

    <ContentPanel>
      <LoadingState v-if="isLoading && !hasRepairTasks" message="Loading repair tasks..." />

      <EmptyState
        v-else-if="!hasRepairTasks"
        title="No repair tasks yet"
        message="Create your first repair task template and attach required inventory parts."
      >
        <template #icon>
          <Wrench :size="28" />
        </template>
        <template #action>
          <ActionButton :to="{ name: 'repair-task-create' }">
            <Plus :size="18" />
            <span>Create repair task</span>
          </ActionButton>
        </template>
      </EmptyState>

      <div v-else class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Task</th>
              <th>Time</th>
              <th>Pricing</th>
              <th>Required parts</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="task in repairTasks" :key="getRepairTaskId(task)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar entity-avatar--service">
                    <Wrench :size="17" />
                  </span>
                  <div>
                    <strong>{{ getRepairTaskName(task) }}</strong>
                    <small>{{ getDescription(task) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <span class="time-pill">
                  <Clock3 :size="14" />
                  {{ formatTime(getValue(task, "timeEstimated", "TimeEstimated")) }}
                </span>
              </td>
              <td>
                <div class="price-stack">
                  <span>
                    <DollarSign :size="14" />
                    Customer {{ formatMoney(getValue(task, "costEstimated", "CostEstimated", 0)) }}
                  </span>
                  <span>
                    <Wrench :size="14" />
                    Labor {{ formatMoney(getValue(task, "technicianCost", "TechnicianCost", 0)) }}
                  </span>
                </div>
              </td>
              <td>
                <div class="parts-preview">
                  <span class="parts-total">
                    <Package :size="14" />
                    {{ formatMoney(getPartsTotal(task)) }}
                  </span>
                  <span
                    v-for="line in getParts(task).slice(0, 2)"
                    :key="`${getRepairTaskId(task)}-${getPartName(line)}`"
                    class="part-chip"
                  >
                    {{ getLineQuantity(line) }}x {{ getPartName(line) }}
                  </span>
                  <span v-if="getParts(task).length > 2" class="part-chip">
                    +{{ getParts(task).length - 2 }} more
                  </span>
                </div>
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'repair-task-edit', params: { id: getRepairTaskId(task) } }"
                    aria-label="Edit repair task"
                  >
                    <Pencil :size="16" />
                  </ActionButton>
                  <ActionButton
                    variant="danger"
                    size="sm"
                    icon-only
                    :disabled="deletingRepairTaskId === getRepairTaskId(task)"
                    aria-label="Delete repair task"
                    @click="handleDelete(task)"
                  >
                    <Trash2
                      v-if="deletingRepairTaskId !== getRepairTaskId(task)"
                      :size="16"
                    />
                    <RefreshCw v-else class="spinning" :size="16" />
                  </ActionButton>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <PaginationBar
        v-if="hasRepairTasks"
        :first-item="firstItemNumber"
        :last-item="lastItemNumber"
        :total-items="totalItems"
        :page="page"
        :total-pages="totalPages"
        :disabled="isLoading"
        @previous="goToPreviousPage"
        @next="goToNextPage"
      />
    </ContentPanel>
  </PageShell>
</template>

<style scoped>
.entity-avatar--service {
  color: #b91c1c;
  background: rgba(239, 68, 68, 0.12);
}

.time-pill,
.parts-total,
.part-chip,
.price-stack span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.time-pill {
  justify-content: center;
  min-width: 92px;
  padding: 7px 10px;
  color: #b91c1c;
  background: #fee2e2;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}

.price-stack,
.parts-preview {
  display: grid;
  gap: 7px;
}

.price-stack span {
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

.parts-preview {
  justify-items: flex-start;
}

.parts-total,
.part-chip {
  min-height: 28px;
  padding: 0 9px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}

.parts-total {
  color: #075985;
  background: #e0f2fe;
}

.part-chip {
  color: #475569;
  background: #f1f5f9;
}
</style>
