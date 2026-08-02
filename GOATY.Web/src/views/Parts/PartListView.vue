<script setup>
import { Package, Pencil, Plus, RefreshCw, Trash2 } from "@lucide/vue";
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
import { deletePart, getParts } from "@/services/parts.service";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();
const parts = ref([]);
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingPartId = ref(null);

const hasParts = computed(() => parts.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => {
  return Math.min(page.value * pageSize.value, totalItems.value);
});

function formatMoney(value) {
  const numberValue = Number(value ?? 0);

  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(numberValue);
}

function getPartId(part) {
  return part.id ?? part.Id;
}

function getPartName(part) {
  return part.name ?? part.Name ?? "Unnamed part";
}

function getErrorMessage(error) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.title ||
    "Something went wrong while loading parts."
  );
}

async function loadParts(targetPage = page.value) {
  isLoading.value = true;

  try {
    const { data } = await getParts({
      Page: targetPage,
      PageSize: pageSize.value,
    });

    parts.value = data.items ?? data.Items ?? [];
    page.value = data.page ?? data.Page ?? targetPage;
    pageSize.value = data.pageSize ?? data.PageSize ?? pageSize.value;
    totalItems.value = data.totalItems ?? data.TotalItems ?? parts.value.length;
    totalPages.value = data.totalPages ?? data.TotalPages ?? 1;
  } catch (error) {
    ui.showErrorToast(getErrorMessage(error), "Unable to load parts");
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
      error.response?.data?.detail ||
        error.response?.data?.title ||
        "Unable to delete this part.",
      "Delete failed"
    );
  } finally {
    deletingPartId.value = null;
  }
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

    <ContentPanel>
      <LoadingState v-if="isLoading && !hasParts" message="Loading parts..." />

      <EmptyState
        v-else-if="!hasParts"
        title="No parts yet"
        message="Add your first inventory part to start building the catalog."
      >
        <template #icon>
          <Package :size="28" />
        </template>
        <template #action>
          <ActionButton :to="{ name: 'part-create' }">
            <Plus :size="18" />
            <span>Create part</span>
          </ActionButton>
        </template>
      </EmptyState>

      <div v-else class="table-wrap">
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
            <tr v-for="part in parts" :key="getPartId(part)">
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
              <td>{{ formatMoney(part.cost ?? part.Cost) }}</td>
              <td>
                <span class="quantity-pill">
                  {{ part.quantity ?? part.Quantity }} in stock
                </span>
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

      <PaginationBar
        v-if="hasParts"
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
.quantity-pill {
  display: inline-flex;
  min-width: 104px;
  justify-content: center;
  padding: 6px 10px;
  color: #166534;
  background: #dcfce7;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}
</style>
