<script setup>
import { Package, Pencil, Plus, RefreshCw, Trash2, X } from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import EntityFilterBar from "@/components/Shared/EntityFilterBar.vue";
import EntityListShell from "@/components/Shared/EntityListShell.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { deletePart, getParts } from "@/services/parts.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMoney } from "@/utils/formatters";
import { matchesSearch, readValue } from "@/utils/objectAccess";

const ui = useUiStore();
const parts = ref([]);
const search = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingPartId = ref(null);
const loadErrorMessage = ref("");

const visibleParts = computed(() => {
  return parts.value.filter((part) =>
    matchesSearch(
      [
        getPartId(part),
        getPartName(part),
        readValue(part, "cost", "Cost"),
        readValue(part, "quantity", "Quantity"),
      ],
      search.value
    )
  );
});

const hasVisibleParts = computed(() => visibleParts.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
const resultLabel = computed(() => {
  if (search.value.trim()) {
    return `${visibleParts.value.length} match${visibleParts.value.length === 1 ? "" : "es"} on this page`;
  }

  return `${totalItems.value} total`;
});
const emptyTitle = computed(() => (search.value.trim() ? "No parts match this search" : "No parts yet"));
const emptyMessage = computed(() =>
  search.value.trim()
    ? "Clear the search or move to another page to keep scanning inventory."
    : "Add your first inventory part to start building the catalog."
);

function getPartId(part) {
  return readValue(part, "id", "Id");
}

function getPartName(part) {
  return readValue(part, "name", "Name", "Unnamed part");
}

async function loadParts(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getParts({
      Page: targetPage,
      PageSize: pageSize.value,
    });
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    parts.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading parts."
    );
  } finally {
    isLoading.value = false;
  }
}

async function handleDelete(part) {
  const partId = getPartId(part);
  const partName = getPartName(part);

  if (!partId) return;

  const shouldDelete = await ui.confirm({
    title: "Delete part?",
    message: `This will permanently remove "${partName}" from the inventory.`,
    confirmText: "Delete part",
    cancelText: "Keep part",
    variant: "danger",
  });

  if (!shouldDelete) return;

  deletingPartId.value = partId;

  try {
    await deletePart(partId);

    ui.showSuccessToast(`"${partName}" was removed from inventory.`, "Part deleted");

    if (parts.value.length === 1 && page.value > 1) {
      await loadParts(page.value - 1);
    } else {
      await loadParts(page.value);
    }
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to delete this part."),
      "Delete failed"
    );
  } finally {
    deletingPartId.value = null;
  }
}

function clearSearch() {
  search.value = "";
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadParts(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadParts(page.value + 1);
}

onMounted(() => {
  loadParts();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Inventory"
      title="Parts"
      subtitle="Track workshop parts, cost, and available stock."
      :icon="Package"
      tone="inventory"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh parts"
          @click="loadParts()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton :to="{ name: 'part-create' }">
          <Plus :size="18" />
          <span>New part</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Parts summary">
      <SummaryCard label="Total items" :value="totalItems" />
      <SummaryCard label="Current page" :value="`${page} / ${totalPages}`" />
      <SummaryCard label="Rows per page" :value="pageSize" />
    </SummaryGrid>

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasVisibleParts"
      :error-message="loadErrorMessage"
      loading-message="Loading parts..."
      :empty-title="emptyTitle"
      :empty-message="emptyMessage"
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadParts()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="search"
          search-placeholder="Search current page by name, ID, cost, or quantity"
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
        <Package :size="28" />
      </template>

      <template #emptyAction>
        <ActionButton v-if="search" variant="secondary" @click="clearSearch">
          <X :size="17" />
          <span>Clear search</span>
        </ActionButton>
        <ActionButton v-else :to="{ name: 'part-create' }">
          <Plus :size="18" />
          <span>Create part</span>
        </ActionButton>
      </template>

      <div class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Cost</th>
              <th>Quantity</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="part in visibleParts" :key="getPartId(part)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar">
                    <Package :size="17" />
                  </span>
                  <div>
                    <strong>{{ getPartName(part) }}</strong>
                    <small>{{ getPartId(part) }}</small>
                  </div>
                </div>
              </td>
              <td>{{ formatMoney(readValue(part, "cost", "Cost", 0)) }}</td>
              <td>
                <StatusChip
                  :label="`${readValue(part, 'quantity', 'Quantity', 0)} in stock`"
                  tone="inventory"
                />
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'part-edit', params: { id: getPartId(part) } }"
                    aria-label="Edit part"
                  >
                    <Pencil :size="16" />
                  </ActionButton>
                  <ActionButton
                    variant="danger"
                    size="sm"
                    icon-only
                    :disabled="deletingPartId === getPartId(part)"
                    aria-label="Delete part"
                    @click="handleDelete(part)"
                  >
                    <Trash2 v-if="deletingPartId !== getPartId(part)" :size="16" />
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
