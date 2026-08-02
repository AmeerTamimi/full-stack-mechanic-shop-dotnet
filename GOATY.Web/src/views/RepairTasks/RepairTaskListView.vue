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
  X,
} from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import EntityFilterBar from "@/components/Shared/EntityFilterBar.vue";
import EntityListShell from "@/components/Shared/EntityListShell.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { deleteRepairTask, getRepairTasks } from "@/services/repairTasks.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMinutes, formatMoney } from "@/utils/formatters";
import { asArray, matchesSearch, readValue } from "@/utils/objectAccess";

const ui = useUiStore();
const repairTasks = ref([]);
const search = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingRepairTaskId = ref(null);
const loadErrorMessage = ref("");

const visibleRepairTasks = computed(() => {
  return repairTasks.value.filter((task) =>
    matchesSearch(
      [
        getRepairTaskId(task),
        getRepairTaskName(task),
        getDescription(task),
        formatMinutes(getValue(task, "timeEstimated", "TimeEstimated")),
        ...getParts(task).flatMap((line) => [getPartName(line), getLineQuantity(line)]),
      ],
      search.value
    )
  );
});

const hasVisibleRepairTasks = computed(() => visibleRepairTasks.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
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
const resultLabel = computed(() => {
  if (search.value.trim()) {
    return `${visibleRepairTasks.value.length} match${visibleRepairTasks.value.length === 1 ? "" : "es"} on this page`;
  }

  return `${totalItems.value} total`;
});
const emptyTitle = computed(() =>
  search.value.trim() ? "No repair tasks match this search" : "No repair tasks yet"
);
const emptyMessage = computed(() =>
  search.value.trim()
    ? "Clear the search or move to another page to keep scanning task templates."
    : "Create your first repair task template and attach required inventory parts."
);

function getValue(source, camelKey, pascalKey, fallback = "") {
  return readValue(source, camelKey, pascalKey, fallback);
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
  return asArray(getValue(task, "parts", "Parts", []));
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

async function loadRepairTasks(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getRepairTasks({
      Page: targetPage,
      PageSize: pageSize.value,
    });
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    repairTasks.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading repair tasks."
    );
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
      getBackendErrorMessage(error, "Unable to delete this repair task."),
      "Delete failed"
    );
  } finally {
    deletingRepairTaskId.value = null;
  }
}

function clearSearch() {
  search.value = "";
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

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasVisibleRepairTasks"
      :error-message="loadErrorMessage"
      loading-message="Loading repair tasks..."
      :empty-title="emptyTitle"
      :empty-message="emptyMessage"
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadRepairTasks()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="search"
          search-placeholder="Search current page by task, description, duration, or part"
          :disabled="isLoading"
          :result-label="resultLabel"
          @clear="clearSearch"
        >
          <template #actions>
            <ActionButton
              v-if="search"
              variant="secondary"
              type="button"
              :disabled="isLoading"
              @click="clearSearch"
            >
              <X :size="17" />
              <span>Clear</span>
            </ActionButton>
          </template>
        </EntityFilterBar>
      </template>

      <template #emptyIcon>
        <Wrench :size="28" />
      </template>

      <template #emptyAction>
        <ActionButton v-if="search" variant="secondary" @click="clearSearch">
          <X :size="17" />
          <span>Clear search</span>
        </ActionButton>
        <ActionButton v-else :to="{ name: 'repair-task-create' }">
          <Plus :size="18" />
          <span>Create repair task</span>
        </ActionButton>
      </template>

      <div class="table-wrap">
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
            <tr v-for="task in visibleRepairTasks" :key="getRepairTaskId(task)">
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
                <StatusChip
                  :label="formatMinutes(getValue(task, 'timeEstimated', 'TimeEstimated'))"
                  tone="service"
                  :icon="Clock3"
                />
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
                  <StatusChip
                    :label="formatMoney(getPartsTotal(task))"
                    tone="info"
                    :icon="Package"
                  />
                  <StatusChip
                    v-for="line in getParts(task).slice(0, 2)"
                    :key="`${getRepairTaskId(task)}-${getPartName(line)}`"
                    :label="`${getLineQuantity(line)}x ${getPartName(line)}`"
                    tone="neutral"
                    size="sm"
                  />
                  <StatusChip
                    v-if="getParts(task).length > 2"
                    :label="`+${getParts(task).length - 2} more`"
                    tone="neutral"
                    size="sm"
                  />
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
    </EntityListShell>
  </PageShell>
</template>

<style scoped>
.entity-avatar--service {
  color: #b91c1c;
  background: rgba(239, 68, 68, 0.12);
}

.price-stack,
.parts-preview {
  display: grid;
  gap: 7px;
}

.price-stack span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

.parts-preview {
  justify-items: flex-start;
}
</style>
