<script setup>
import {
  ChevronLeft,
  ChevronRight,
  Package,
  Pencil,
  Plus,
  RefreshCw,
  Trash2,
} from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
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

    parts.value = data.items;
    page.value = data.page;
    pageSize.value = data.pageSize;
    totalItems.value = data.totalItems;
    totalPages.value = data.totalPages || 1;
  } catch (error) {
    ui.showErrorToast(getErrorMessage(error), "Unable to load parts");
  } finally {
    isLoading.value = false;
  }
}

async function handleDelete(part) {
  const partName = part.name ?? part.Name ?? "this part";
  const partId = part.id ?? part.Id;

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
  <main class="parts-page">
    <section class="parts-shell">
      <header class="page-header">
        <div>
          <p class="page-eyebrow">Inventory</p>
          <h1>Parts</h1>
          <p class="page-subtitle">
            Track workshop parts, cost, and available stock.
          </p>
        </div>

        <div class="header-actions">
          <button
            class="icon-button"
            type="button"
            :disabled="isLoading"
            aria-label="Refresh parts"
            @click="loadParts()"
          >
            <RefreshCw :class="{ spinning: isLoading }" :size="18" />
          </button>

          <RouterLink class="create-button" :to="{ name: 'part-create' }">
            <Plus :size="18" />
            <span>New part</span>
          </RouterLink>
        </div>
      </header>

      <section class="summary-grid" aria-label="Parts summary">
        <article class="summary-card">
          <span>Total items</span>
          <strong>{{ totalItems }}</strong>
        </article>
        <article class="summary-card">
          <span>Current page</span>
          <strong>{{ page }} / {{ totalPages }}</strong>
        </article>
        <article class="summary-card">
          <span>Rows per page</span>
          <strong>{{ pageSize }}</strong>
        </article>
      </section>

      <section class="content-panel">
        <div v-if="isLoading && !hasParts" class="state-message">
          <RefreshCw class="spinning" :size="18" />
          <span>Loading parts...</span>
        </div>

        <div v-else-if="!hasParts" class="empty-state">
          <div class="empty-icon">
            <Package :size="28" />
          </div>
          <h2>No parts yet</h2>
          <p>Add your first inventory part to start building the catalog.</p>
          <RouterLink class="create-button" :to="{ name: 'part-create' }">
            <Plus :size="18" />
            <span>Create part</span>
          </RouterLink>
        </div>

        <div v-else class="table-wrap">
          <table class="parts-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Cost</th>
                <th>Quantity</th>
                <th class="actions-column">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="part in parts" :key="part.id ?? part.Id">
                <td>
                  <div class="part-name">
                    <span class="part-avatar">
                      <Package :size="17" />
                    </span>
                    <div>
                      <strong>{{ part.name ?? part.Name }}</strong>
                      <small>{{ part.id ?? part.Id }}</small>
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
                    <RouterLink
                      class="row-action"
                      :to="{ name: 'part-edit', params: { id: part.id ?? part.Id } }"
                      aria-label="Edit part"
                    >
                      <Pencil :size="16" />
                    </RouterLink>
                    <button
                      class="row-action row-action--danger"
                      type="button"
                      :disabled="deletingPartId === (part.id ?? part.Id)"
                      aria-label="Delete part"
                      @click="handleDelete(part)"
                    >
                      <Trash2 v-if="deletingPartId !== (part.id ?? part.Id)" :size="16" />
                      <RefreshCw v-else class="spinning" :size="16" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <footer v-if="hasParts" class="pagination-bar">
          <p>
            Showing {{ firstItemNumber }}-{{ lastItemNumber }} of {{ totalItems }}
          </p>
          <div class="pagination-actions">
            <button
              class="pager-button"
              type="button"
              :disabled="page <= 1 || isLoading"
              @click="goToPreviousPage"
            >
              <ChevronLeft :size="17" />
              <span>Previous</span>
            </button>
            <button
              class="pager-button"
              type="button"
              :disabled="page >= totalPages || isLoading"
              @click="goToNextPage"
            >
              <span>Next</span>
              <ChevronRight :size="17" />
            </button>
          </div>
        </footer>
      </section>
    </section>
  </main>
</template>

<style scoped>
.parts-page {
  min-height: 100vh;
  padding: 32px;
  color: #111827;
  background:
    radial-gradient(circle at top left, rgba(245, 158, 11, 0.13), transparent 28%),
    linear-gradient(135deg, #f8fafc 0%, #edf2f7 100%);
}

.parts-shell {
  width: min(1180px, 100%);
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

.header-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.icon-button,
.create-button,
.row-action,
.pager-button {
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

.icon-button {
  width: 44px;
  height: 44px;
  color: #374151;
  background: #fff;
  border: 1px solid #e5e7eb;
}

.create-button {
  min-height: 44px;
  gap: 8px;
  padding: 0 15px;
  color: #fff;
  background: linear-gradient(135deg, #111827, #263142);
  box-shadow: 0 14px 28px rgba(15, 23, 42, 0.16);
}

.icon-button:hover:not(:disabled),
.create-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 18px 30px rgba(15, 23, 42, 0.18);
}

button:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 16px;
}

.summary-card {
  padding: 18px;
  background: rgba(255, 255, 255, 0.86);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 10px 26px rgba(15, 23, 42, 0.06);
}

.summary-card span {
  display: block;
  color: #6b7280;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.summary-card strong {
  display: block;
  margin-top: 8px;
  color: #111827;
  font-size: 26px;
  line-height: 1;
}

.content-panel {
  overflow: hidden;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 20px 48px rgba(15, 23, 42, 0.1);
}

.state-message {
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 18px;
  padding: 14px;
  color: #475569;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-weight: 700;
}

.state-message--error {
  color: #991b1b;
  background: #fff1f2;
  border-color: #fecdd3;
}

.empty-state {
  display: grid;
  justify-items: center;
  gap: 12px;
  padding: 64px 24px;
  text-align: center;
}

.empty-icon {
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.12);
  border-radius: 8px;
}

.empty-state h2 {
  margin: 0;
  color: #111827;
  font-size: 24px;
}

.empty-state p {
  max-width: 390px;
  margin: 0 0 8px;
  color: #6b7280;
}

.table-wrap {
  overflow-x: auto;
}

.parts-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.parts-table th {
  padding: 15px 18px;
  color: #6b7280;
  background: #f8fafc;
  border-bottom: 1px solid #e5e7eb;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
  white-space: nowrap;
}

.parts-table td {
  padding: 16px 18px;
  border-bottom: 1px solid #eef2f7;
  vertical-align: middle;
}

.parts-table tbody tr:hover {
  background: rgba(245, 158, 11, 0.04);
}

.part-name {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 280px;
}

.part-avatar {
  display: inline-grid;
  place-items: center;
  width: 38px;
  height: 38px;
  flex: 0 0 38px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.13);
  border-radius: 8px;
}

.part-name strong {
  display: block;
  color: #111827;
  font-size: 15px;
}

.part-name small {
  display: block;
  max-width: 300px;
  margin-top: 3px;
  overflow: hidden;
  color: #9ca3af;
  font-size: 12px;
  text-overflow: ellipsis;
  white-space: nowrap;
}

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

.actions-column {
  width: 126px;
  text-align: right;
}

.row-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.row-action {
  width: 36px;
  height: 36px;
  color: #374151;
  background: #f3f4f6;
}

.row-action:hover:not(:disabled) {
  color: #111827;
  background: #e5e7eb;
}

.row-action--danger {
  color: #b91c1c;
}

.row-action--danger:hover:not(:disabled) {
  color: #991b1b;
  background: #fee2e2;
}

.pagination-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 16px 18px;
  color: #6b7280;
  background: #fff;
  font-size: 13px;
  font-weight: 700;
}

.pagination-bar p {
  margin: 0;
}

.pagination-actions {
  display: flex;
  gap: 8px;
}

.pager-button {
  gap: 7px;
  min-height: 38px;
  padding: 0 12px;
  color: #374151;
  background: #f3f4f6;
}

.pager-button:hover:not(:disabled) {
  color: #111827;
  background: #e5e7eb;
}

.spinning {
  animation: spin 850ms linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 760px) {
  .parts-page {
    padding: 18px;
  }

  .page-header,
  .pagination-bar {
    align-items: stretch;
    flex-direction: column;
  }

  .header-actions,
  .pagination-actions {
    width: 100%;
  }

  .create-button,
  .pager-button {
    flex: 1;
  }

  .summary-grid {
    grid-template-columns: 1fr;
  }
}
</style>
