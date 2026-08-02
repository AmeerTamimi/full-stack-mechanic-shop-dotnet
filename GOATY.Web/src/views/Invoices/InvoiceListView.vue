<script setup>
import { Download, Eye, FileText, ReceiptText, RefreshCw, Search, X } from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import EntityFilterBar from "@/components/Shared/EntityFilterBar.vue";
import EntityListShell from "@/components/Shared/EntityListShell.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { getInvoicePdf, getInvoices } from "@/services/invoices.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { formatMoney } from "@/utils/formatters";
import {
  downloadInvoicePdfFromData,
  formatInvoiceCode,
  formatInvoiceIssuedAt,
  formatInvoicePaidAt,
  getInvoiceId,
  getInvoiceItems,
  getInvoiceStatus,
  getInvoiceStatusLabel,
  getInvoiceStatusTone,
  getInvoiceWorkOrderId,
  INVOICE_STATUSES,
} from "@/utils/invoices";
import { matchesSearch, readValue } from "@/utils/objectAccess";
import { formatWorkOrderCode } from "@/utils/workOrders";

const ui = useUiStore();
const invoices = ref([]);
const search = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const loadErrorMessage = ref("");
const downloadingInvoiceId = ref(null);

const visibleInvoices = computed(() => {
  return invoices.value.filter((invoice) =>
    matchesSearch(
      [
        getInvoiceId(invoice),
        getInvoiceWorkOrderId(invoice),
        getInvoiceStatusLabel(getInvoiceStatus(invoice)),
        readValue(invoice, "total", "Total"),
      ],
      search.value
    )
  );
});
const hasVisibleInvoices = computed(() => visibleInvoices.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
const unpaidCount = computed(() => {
  return invoices.value.filter((invoice) => getInvoiceStatus(invoice) === INVOICE_STATUSES.notPaid).length;
});
const paidCount = computed(() => {
  return invoices.value.filter((invoice) => getInvoiceStatus(invoice) === INVOICE_STATUSES.paid).length;
});
const refundedCount = computed(() => {
  return invoices.value.filter((invoice) => getInvoiceStatus(invoice) === INVOICE_STATUSES.refunded).length;
});
const resultLabel = computed(() => {
  if (search.value.trim()) {
    return `${visibleInvoices.value.length} match${visibleInvoices.value.length === 1 ? "" : "es"} on this page`;
  }

  return `${totalItems.value} total`;
});
const emptyTitle = computed(() => (search.value.trim() ? "No invoices match this search" : "No invoices yet"));
const emptyMessage = computed(() =>
  search.value.trim()
    ? "Clear the search or move to another page to keep scanning invoices."
    : "Invoices appear here after completed work orders are billed."
);

async function loadInvoices(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getInvoices({
      Page: targetPage,
      PageSize: pageSize.value,
    });
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    invoices.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading invoices."
    );
  } finally {
    isLoading.value = false;
  }
}

async function handleDownload(invoice) {
  const invoiceId = getInvoiceId(invoice);

  if (!invoiceId) return;

  downloadingInvoiceId.value = invoiceId;

  try {
    const { data } = await getInvoicePdf(invoiceId);
    downloadInvoicePdfFromData(data, `${formatInvoiceCode(invoiceId)}.pdf`);
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to download this invoice PDF."),
      "Download failed"
    );
  } finally {
    downloadingInvoiceId.value = null;
  }
}

function clearSearch() {
  search.value = "";
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadInvoices(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadInvoices(page.value + 1);
}

onMounted(() => {
  loadInvoices();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Billing"
      title="Invoices"
      subtitle="Review issued invoices, payment state, PDF exports, and linked work orders."
      :icon="ReceiptText"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh invoices"
          @click="loadInvoices()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Invoice summary">
      <SummaryCard label="Total invoices" :value="totalItems" />
      <SummaryCard label="Unpaid on page" :value="unpaidCount" />
      <SummaryCard label="Paid on page" :value="paidCount" />
      <SummaryCard label="Refunded on page" :value="refundedCount" />
    </SummaryGrid>

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasVisibleInvoices"
      :error-message="loadErrorMessage"
      loading-message="Loading invoices..."
      :empty-title="emptyTitle"
      :empty-message="emptyMessage"
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadInvoices()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="search"
          search-placeholder="Search current page by invoice, work order, status, or total"
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
            <ActionButton variant="secondary" type="submit" :disabled="isLoading">
              <Search :size="17" />
              <span>Search</span>
            </ActionButton>
          </template>
        </EntityFilterBar>
      </template>

      <template #emptyIcon>
        <ReceiptText :size="28" />
      </template>

      <div class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Invoice</th>
              <th>Status</th>
              <th>Issued / paid</th>
              <th>Work order</th>
              <th>Items</th>
              <th>Total</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="invoice in visibleInvoices" :key="getInvoiceId(invoice)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar entity-avatar--invoice">
                    <ReceiptText :size="17" />
                  </span>
                  <div>
                    <strong>{{ formatInvoiceCode(getInvoiceId(invoice)) }}</strong>
                    <small>{{ getInvoiceId(invoice) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <StatusChip
                  :label="getInvoiceStatusLabel(getInvoiceStatus(invoice))"
                  :tone="getInvoiceStatusTone(getInvoiceStatus(invoice))"
                />
              </td>
              <td>
                <div class="info-stack">
                  <span>{{ formatInvoiceIssuedAt(invoice) }}</span>
                  <small>{{ formatInvoicePaidAt(invoice) }}</small>
                </div>
              </td>
              <td>
                <ActionButton
                  variant="ghost"
                  size="sm"
                  :to="{ name: 'work-order-details', params: { id: getInvoiceWorkOrderId(invoice) } }"
                >
                  <FileText :size="16" />
                  <span>{{ formatWorkOrderCode(getInvoiceWorkOrderId(invoice)) }}</span>
                </ActionButton>
              </td>
              <td>
                <StatusChip
                  :label="`${getInvoiceItems(invoice).length} items`"
                  tone="service"
                />
              </td>
              <td>
                <strong>{{ formatMoney(readValue(invoice, "total", "Total", 0)) }}</strong>
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'invoice-details', params: { id: getInvoiceId(invoice) } }"
                    aria-label="Open invoice"
                  >
                    <Eye :size="16" />
                  </ActionButton>
                  <ActionButton
                    variant="secondary"
                    size="sm"
                    icon-only
                    :disabled="downloadingInvoiceId === getInvoiceId(invoice)"
                    aria-label="Download invoice PDF"
                    @click="handleDownload(invoice)"
                  >
                    <RefreshCw
                      v-if="downloadingInvoiceId === getInvoiceId(invoice)"
                      class="spinning"
                      :size="16"
                    />
                    <Download v-else :size="16" />
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
.entity-avatar--invoice {
  color: #0f766e;
  background: #ccfbf1;
}

.info-stack {
  display: grid;
  gap: 5px;
}

.info-stack span,
.info-stack small {
  max-width: 220px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.info-stack span {
  color: #111827;
  font-weight: 850;
}

.info-stack small {
  color: #64748b;
  font-weight: 750;
}
</style>
