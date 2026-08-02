<script setup>
import {
  ArrowLeft,
  CheckCircle2,
  ClipboardList,
  DollarSign,
  Download,
  FileText,
  LoaderCircle,
  ReceiptText,
  RefreshCw,
  RotateCcw,
  Wrench,
} from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import ErrorState from "@/components/Shared/ErrorState.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import {
  getInvoice,
  getInvoicePdf,
  refundInvoice as refundInvoiceRequest,
  settleInvoice as settleInvoiceRequest,
} from "@/services/invoices.service";
import { getWorkOrder } from "@/services/workOrders.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, getBackendErrorTitle } from "@/utils/api";
import { formatMoney } from "@/utils/formatters";
import {
  canRefundInvoice,
  canSettleInvoice,
  downloadInvoicePdfFromData,
  formatInvoiceCode,
  formatInvoiceIssuedAt,
  formatInvoicePaidAt,
  formatInvoiceTotals,
  getInvoiceId,
  getInvoiceItemSourceLabel,
  getInvoiceItems,
  getInvoiceStatus,
  getInvoiceStatusLabel,
  getInvoiceStatusTone,
  getInvoiceWorkOrderId,
} from "@/utils/invoices";
import { readValue } from "@/utils/objectAccess";
import {
  formatWorkOrderCode,
  getCustomer,
  getCustomerName,
  getRepairTaskFromLine,
  getRepairTaskId,
  getRepairTaskName,
  getVehicle,
  getVehicleLabel,
  getVehiclePlate,
  getWorkOrderRepairTaskLines,
} from "@/utils/workOrders";

const route = useRoute();
const ui = useUiStore();

const invoice = ref(null);
const workOrder = ref(null);
const isLoading = ref(false);
const loadErrorMessage = ref("");
const savingAction = ref("");

const invoiceId = computed(() => route.params.id?.toString());
const invoiceStatus = computed(() => getInvoiceStatus(invoice.value));
const totals = computed(() => formatInvoiceTotals(invoice.value));
const invoiceItems = computed(() => getInvoiceItems(invoice.value));
const workOrderId = computed(() => getInvoiceWorkOrderId(invoice.value));
const workOrderTaskLines = computed(() => getWorkOrderRepairTaskLines(workOrder.value));
const workOrderCustomer = computed(() => getCustomer(workOrder.value));
const workOrderVehicle = computed(() => getVehicle(workOrder.value));

async function loadInvoiceDetails() {
  if (!invoiceId.value) {
    loadErrorMessage.value = "Missing invoice id.";
    return;
  }

  isLoading.value = true;
  loadErrorMessage.value = "";
  workOrder.value = null;

  try {
    const { data } = await getInvoice(invoiceId.value);
    invoice.value = data;

    const linkedWorkOrderId = getInvoiceWorkOrderId(data);

    if (linkedWorkOrderId) {
      try {
        const workOrderResponse = await getWorkOrder(linkedWorkOrderId);
        workOrder.value = workOrderResponse.data;
      } catch {
        workOrder.value = null;
      }
    }
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Unable to load this invoice."
    );
  } finally {
    isLoading.value = false;
  }
}

function getInvoiceItemName(item) {
  const repairTaskId = readValue(item, "repairTaskId", "RepairTaskId");

  if (repairTaskId) {
    const normalizedRepairTaskId = String(repairTaskId).toLowerCase();
    const matchingLine = workOrderTaskLines.value.find((line) => {
      return String(getRepairTaskId(getRepairTaskFromLine(line))).toLowerCase() === normalizedRepairTaskId;
    });

    const repairTaskName = getRepairTaskName(getRepairTaskFromLine(matchingLine));

    if (repairTaskName && repairTaskName !== "Unnamed task") {
      return repairTaskName;
    }

    return `Repair task ${String(repairTaskId).slice(0, 8)}`;
  }

  const partId = readValue(item, "partId", "PartId");

  if (partId) {
    return `Part ${String(partId).slice(0, 8)}`;
  }

  return "Invoice line";
}

async function handleDownload() {
  if (!invoiceId.value) return;

  savingAction.value = "download";

  try {
    const { data } = await getInvoicePdf(invoiceId.value);
    downloadInvoicePdfFromData(data, `${formatInvoiceCode(invoiceId.value)}.pdf`);
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to download this invoice PDF."),
      "Download failed"
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleSettle() {
  const shouldSettle = await ui.confirm({
    title: "Settle invoice?",
    message: `${formatInvoiceCode(invoiceId.value)} will be marked as paid.`,
    confirmText: "Settle invoice",
    cancelText: "Keep unpaid",
    variant: "primary",
  });

  if (!shouldSettle) return;

  savingAction.value = "settle";

  try {
    await settleInvoiceRequest(invoiceId.value);
    ui.showSuccessToast(`${formatInvoiceCode(invoiceId.value)} was marked as paid.`, "Invoice settled");
    await loadInvoiceDetails();
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to settle this invoice."),
      getBackendErrorTitle(error, "Settle failed", {
        conflictTitle: "Invoice conflict",
      })
    );
  } finally {
    savingAction.value = "";
  }
}

async function handleRefund() {
  const shouldRefund = await ui.confirm({
    title: "Refund invoice?",
    message: `${formatInvoiceCode(invoiceId.value)} will be marked as refunded.`,
    confirmText: "Refund invoice",
    cancelText: "Keep paid",
    variant: "danger",
  });

  if (!shouldRefund) return;

  savingAction.value = "refund";

  try {
    await refundInvoiceRequest(invoiceId.value);
    ui.showSuccessToast(`${formatInvoiceCode(invoiceId.value)} was refunded.`, "Invoice refunded");
    await loadInvoiceDetails();
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to refund this invoice."),
      getBackendErrorTitle(error, "Refund failed", {
        conflictTitle: "Invoice conflict",
      })
    );
  } finally {
    savingAction.value = "";
  }
}

onMounted(() => {
  loadInvoiceDetails();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Billing"
      :title="invoice ? formatInvoiceCode(getInvoiceId(invoice)) : 'Invoice details'"
      subtitle="Review invoice totals, line items, source work order, and payment state."
      :icon="ReceiptText"
      tone="dashboard"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'invoices' }">
          <ArrowLeft :size="18" />
          <span>Back to invoices</span>
        </ActionButton>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh invoice"
          @click="loadInvoiceDetails"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>
      </template>
    </PageHeader>

    <LoadingState v-if="isLoading" message="Loading invoice..." />

    <ErrorState
      v-else-if="loadErrorMessage"
      title="Unable to load invoice"
      :message="loadErrorMessage"
      @retry="loadInvoiceDetails"
    />

    <template v-else-if="invoice">
      <SummaryGrid aria-label="Invoice summary">
        <SummaryCard label="Status" :value="getInvoiceStatusLabel(invoiceStatus)" />
        <SummaryCard label="Subtotal" :value="totals.subTotal" />
        <SummaryCard label="Tax" :value="totals.tax" />
        <SummaryCard label="Total" :value="totals.total" />
      </SummaryGrid>

      <section class="details-grid">
        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Invoice</span>
              <h2>Payment state</h2>
            </div>
            <StatusChip
              :label="getInvoiceStatusLabel(invoiceStatus)"
              :tone="getInvoiceStatusTone(invoiceStatus)"
            />
          </div>

          <div class="detail-list">
            <div class="detail-row">
              <span>Issued</span>
              <strong>{{ formatInvoiceIssuedAt(invoice) }}</strong>
            </div>
            <div class="detail-row">
              <span>Paid</span>
              <strong>{{ formatInvoicePaidAt(invoice) }}</strong>
            </div>
            <div class="detail-row">
              <span>Discount</span>
              <strong>{{ totals.discount }}</strong>
            </div>
          </div>
        </ContentPanel>

        <ContentPanel class="details-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Source</span>
              <h2>Work order</h2>
            </div>
            <StatusChip :label="formatWorkOrderCode(workOrderId)" tone="service" :icon="ClipboardList" />
          </div>

          <div class="detail-list">
            <div class="detail-row">
              <span>Work order</span>
              <strong>{{ formatWorkOrderCode(workOrderId) }}</strong>
            </div>
            <div class="detail-row">
              <span>Customer</span>
              <strong>{{ getCustomerName(workOrderCustomer) }}</strong>
            </div>
            <div class="detail-row">
              <span>Vehicle</span>
              <strong>{{ getVehicleLabel(workOrderVehicle) }}</strong>
              <small v-if="getVehiclePlate(workOrderVehicle)">{{ getVehiclePlate(workOrderVehicle) }}</small>
            </div>
          </div>

          <ActionButton
            class="panel-action"
            variant="secondary"
            :to="{ name: 'work-order-details', params: { id: workOrderId } }"
          >
            <FileText :size="18" />
            <span>Open work order</span>
          </ActionButton>
        </ContentPanel>
      </section>

      <ContentPanel class="details-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">Money lane</span>
            <h2>Invoice totals</h2>
          </div>
          <StatusChip :label="totals.total" tone="success" :icon="DollarSign" />
        </div>

        <div class="totals-grid">
          <div>
            <span>Subtotal</span>
            <strong>{{ totals.subTotal }}</strong>
          </div>
          <div>
            <span>Tax</span>
            <strong>{{ totals.tax }}</strong>
          </div>
          <div>
            <span>Discount</span>
            <strong>{{ totals.discount }}</strong>
          </div>
          <div>
            <span>Total</span>
            <strong>{{ totals.total }}</strong>
          </div>
        </div>
      </ContentPanel>

      <ContentPanel class="details-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">Line items</span>
            <h2>Invoice items</h2>
          </div>
          <StatusChip :label="`${invoiceItems.length} items`" tone="service" :icon="Wrench" />
        </div>

        <div class="table-wrap">
          <table class="data-table">
            <thead>
              <tr>
                <th>Item</th>
                <th>Source</th>
                <th>Quantity</th>
                <th>Technician</th>
                <th>Unit price</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in invoiceItems" :key="readValue(item, 'id', 'Id')">
                <td>
                  <strong>{{ getInvoiceItemName(item) }}</strong>
                  <small class="muted-id">{{ readValue(item, "id", "Id") }}</small>
                </td>
                <td>
                  <StatusChip :label="getInvoiceItemSourceLabel(item)" tone="neutral" />
                </td>
                <td>{{ readValue(item, "quantity", "Quantity", 0) }}</td>
                <td>{{ formatMoney(readValue(item, "technicianCost", "TechnicianCost", 0)) }}</td>
                <td>{{ formatMoney(readValue(item, "unitPrice", "UnitPrice", 0)) }}</td>
                <td>
                  <strong>{{ formatMoney(readValue(item, "total", "Total", 0)) }}</strong>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </ContentPanel>

      <ContentPanel class="details-panel">
        <div class="panel-heading">
          <div>
            <span class="panel-kicker">Actions</span>
            <h2>Payment and PDF</h2>
          </div>
          <StatusChip
            :label="getInvoiceStatusLabel(invoiceStatus)"
            :tone="getInvoiceStatusTone(invoiceStatus)"
          />
        </div>

        <div class="action-strip">
          <ActionButton :disabled="Boolean(savingAction)" @click="handleDownload">
            <LoaderCircle v-if="savingAction === 'download'" class="spinning" :size="18" />
            <Download v-else :size="18" />
            <span>Download PDF</span>
          </ActionButton>

          <ActionButton
            v-if="canSettleInvoice(invoice)"
            variant="secondary"
            :disabled="Boolean(savingAction)"
            @click="handleSettle"
          >
            <LoaderCircle v-if="savingAction === 'settle'" class="spinning" :size="18" />
            <CheckCircle2 v-else :size="18" />
            <span>Settle invoice</span>
          </ActionButton>

          <ActionButton
            v-if="canRefundInvoice(invoice)"
            variant="danger"
            :disabled="Boolean(savingAction)"
            @click="handleRefund"
          >
            <LoaderCircle v-if="savingAction === 'refund'" class="spinning" :size="18" />
            <RotateCcw v-else :size="18" />
            <span>Refund invoice</span>
          </ActionButton>
        </div>
      </ContentPanel>
    </template>
  </PageShell>
</template>

<style scoped>
.details-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.details-panel {
  padding: 22px;
  margin-bottom: 16px;
}

.panel-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
}

.panel-kicker {
  display: block;
  margin-bottom: 6px;
  color: #0f766e;
  font-size: 11px;
  font-weight: 900;
  text-transform: uppercase;
}

.panel-heading h2 {
  margin: 0;
  color: #111827;
  font-size: 20px;
}

.panel-heading > div {
  min-width: 0;
}

.detail-list {
  display: grid;
  gap: 12px;
}

.detail-row {
  display: grid;
  gap: 5px;
  padding-bottom: 12px;
  border-bottom: 1px solid #e5e7eb;
}

.detail-row:last-child {
  padding-bottom: 0;
  border-bottom: 0;
}

.detail-row span,
.detail-row small {
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

.detail-row strong {
  min-width: 0;
  overflow-wrap: anywhere;
  color: #111827;
  font-size: 15px;
}

.panel-action {
  margin-top: 18px;
}

.totals-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 12px;
}

.totals-grid > div {
  display: grid;
  gap: 6px;
  padding: 16px;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.totals-grid span {
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.totals-grid strong {
  min-width: 0;
  overflow-wrap: anywhere;
  color: #111827;
  font-size: 22px;
}

.muted-id {
  display: block;
  max-width: 220px;
  margin-top: 4px;
  overflow: hidden;
  color: #64748b;
  font-size: 12px;
  font-weight: 700;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.action-strip {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.action-strip :deep(.action-button) {
  max-width: 100%;
}

@media (max-width: 880px) {
  .details-grid,
  .totals-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 720px) {
  .panel-heading,
  .action-strip {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
