<script setup>
import {
  Activity,
  ArrowRight,
  CalendarDays,
  Car,
  CheckCircle2,
  ClipboardList,
  Clock3,
  DollarSign,
  Gauge,
  Package,
  Percent,
  ReceiptText,
  RefreshCw,
  TrendingUp,
  UserRound,
  Users,
  Wrench,
  XCircle,
} from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getDashboard } from "@/services/dashboard.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage } from "@/utils/api";
import { formatDate as formatDateValue, formatMoney, formatPercent } from "@/utils/formatters";
import { readValue } from "@/utils/objectAccess";

const ui = useUiStore();
const isLoading = ref(false);
const hasLoaded = ref(false);
const selectedDay = ref(getTodayInputValue());
const timezone = ref(getBrowserTimeZone());
const dashboard = ref(createEmptyDashboard());

const displayedDay = computed(() => {
  return formatDateValue(dashboard.value.day || selectedDay.value, { fallback: "Today" });
});

const hasOrders = computed(() => dashboard.value.totalOrders > 0);

const heroMetrics = computed(() => [
  {
    label: "Orders today",
    value: dashboard.value.totalOrders,
    helper: `${dashboard.value.uniqueVehicles} vehicles booked`,
    icon: ClipboardList,
    tone: "orders",
  },
  {
    label: "Revenue",
    value: formatMoney(dashboard.value.totalRevenue),
    helper: `${formatMoney(dashboard.value.avgRevenuePerOrder)} avg per order`,
    icon: DollarSign,
    tone: "revenue",
  },
  {
    label: "Net profit",
    value: formatMoney(dashboard.value.netProfit),
    helper: `${formatPercent(dashboard.value.profitMargin)} margin`,
    icon: TrendingUp,
    tone: dashboard.value.netProfit >= 0 ? "profit" : "loss",
  },
]);

const statusItems = computed(() => [
  {
    label: "Scheduled",
    value: dashboard.value.totalScheduled,
    icon: CalendarDays,
    color: "#f59e0b",
  },
  {
    label: "In progress",
    value: dashboard.value.totalInProgress,
    icon: Clock3,
    color: "#38bdf8",
  },
  {
    label: "Completed",
    value: dashboard.value.totalCompleted,
    icon: CheckCircle2,
    color: "#22c55e",
  },
  {
    label: "Cancelled",
    value: dashboard.value.totalCancelled,
    icon: XCircle,
    color: "#ef4444",
  },
]);

const moneyBreakdown = computed(() => [
  {
    label: "Revenue",
    value: dashboard.value.totalRevenue,
    formatted: formatMoney(dashboard.value.totalRevenue),
    icon: DollarSign,
  },
  {
    label: "Parts cost",
    value: dashboard.value.totalPartsCost,
    formatted: formatMoney(dashboard.value.totalPartsCost),
    icon: Package,
  },
  {
    label: "Technicians cost",
    value: dashboard.value.totalTechniciansCost,
    formatted: formatMoney(dashboard.value.totalTechniciansCost),
    icon: Wrench,
  },
]);

const ratioCards = computed(() => [
  {
    label: "Completion",
    value: dashboard.value.completionRate,
    icon: CheckCircle2,
    accent: "#22c55e",
  },
  {
    label: "Cancellation",
    value: dashboard.value.cancellationRate,
    icon: XCircle,
    accent: "#ef4444",
  },
  {
    label: "Parts ratio",
    value: dashboard.value.partsCostRatio,
    icon: Package,
    accent: "#f59e0b",
  },
  {
    label: "Labor ratio",
    value: dashboard.value.laborCostRatio,
    icon: Wrench,
    accent: "#38bdf8",
  },
]);

const moduleCards = [
  {
    title: "Schedule",
    text: "Daily bay timeline and technician workload.",
    routeName: "schedule",
    icon: CalendarDays,
    tone: "schedule",
  },
  {
    title: "Work orders",
    text: "Create and manage service jobs through their lifecycle.",
    routeName: "work-orders",
    icon: ClipboardList,
    tone: "orders",
  },
  {
    title: "Invoices",
    text: "Issue, settle, refund, and download billing records.",
    routeName: "invoices",
    icon: ReceiptText,
    tone: "billing",
  },
  {
    title: "Parts",
    text: "Inventory list, create, edit, and delete.",
    routeName: "parts",
    icon: Package,
    tone: "inventory",
  },
  {
    title: "Repair tasks",
    text: "Reusable service templates with labor and required parts.",
    routeName: "repair-tasks",
    icon: Wrench,
    tone: "service",
  },
  {
    title: "Employees",
    text: "Manage managers and technicians.",
    routeName: "employees",
    icon: Users,
    tone: "people",
  },
  {
    title: "Customers",
    text: "Customer records with their vehicles.",
    routeName: "customers",
    icon: UserRound,
    tone: "customers",
  },
];

function getTodayInputValue() {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, "0");
  const day = String(now.getDate()).padStart(2, "0");

  return `${now.getFullYear()}-${month}-${day}`;
}

function getBrowserTimeZone() {
  return Intl.DateTimeFormat().resolvedOptions().timeZone || "UTC";
}

function createEmptyDashboard() {
  return {
    day: selectedDay.value,
    totalOrders: 0,
    totalScheduled: 0,
    totalInProgress: 0,
    totalCompleted: 0,
    totalCancelled: 0,
    totalRevenue: 0,
    totalPartsCost: 0,
    totalTechniciansCost: 0,
    netProfit: 0,
    uniqueVehicles: 0,
    uniqueCustomers: 0,
    profitMargin: 0,
    completionRate: 0,
    cancellationRate: 0,
    avgRevenuePerOrder: 0,
    ordersPerVehicle: 0,
    partsCostRatio: 0,
    laborCostRatio: 0,
  };
}

function normalizeDashboard(data) {
  return {
    day: readValue(data, "day", "Day", selectedDay.value),
    totalOrders: Number(readValue(data, "totalOrders", "TotalOrders")),
    totalScheduled: Number(readValue(data, "totalScheduled", "TotalScheduled")),
    totalInProgress: Number(readValue(data, "totalInProgress", "TotalInProgress")),
    totalCompleted: Number(readValue(data, "totalCompleted", "TotalCompleted")),
    totalCancelled: Number(readValue(data, "totalCancelled", "TotalCancelled")),
    totalRevenue: Number(readValue(data, "totalRevenue", "TotalRevenue")),
    totalPartsCost: Number(readValue(data, "totalPartsCost", "TotalPartsCost")),
    totalTechniciansCost: Number(
      readValue(data, "totalTechniciansCost", "TotalTechniciansCost")
    ),
    netProfit: Number(readValue(data, "netProfit", "NetProfit")),
    uniqueVehicles: Number(readValue(data, "uniqueVehicles", "UniqueVehicles")),
    uniqueCustomers: Number(readValue(data, "uniqueCustomers", "UniqueCustomers")),
    profitMargin: Number(readValue(data, "profitMargin", "ProfitMargin")),
    completionRate: Number(readValue(data, "completionRate", "CompletionRate")),
    cancellationRate: Number(readValue(data, "cancellationRate", "CancellationRate")),
    avgRevenuePerOrder: Number(
      data?.avgRevenuePerOrder ??
        data?.avgRevenuPerOrder ??
        data?.AvgRevenuePerOrder ??
        data?.AvgRevenuPerOrder ??
        0
    ),
    ordersPerVehicle: Number(readValue(data, "ordersPerVehicle", "OrdersPerVehicle")),
    partsCostRatio: Number(readValue(data, "partsCostRatio", "PartsCostRatio")),
    laborCostRatio: Number(readValue(data, "laborCostRatio", "LaborCostRatio")),
  };
}

function getStatusWidth(value) {
  if (!dashboard.value.totalOrders) return "0%";

  const width = (Number(value || 0) / dashboard.value.totalOrders) * 100;

  return `${Math.min(100, Math.max(0, width))}%`;
}

function getRatioProgress(value) {
  return `${Math.min(100, Math.max(0, Number(value || 0)))}%`;
}

async function loadDashboard() {
  isLoading.value = true;

  try {
    const { data } = await getDashboard({
      Day: selectedDay.value,
      TimeZone: timezone.value || "UTC",
    });

    dashboard.value = normalizeDashboard(data);
    hasLoaded.value = true;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to load dashboard data. Please try again."),
      "Dashboard failed"
    );
  } finally {
    isLoading.value = false;
  }
}

function handleDateChange() {
  loadDashboard();
}

onMounted(() => {
  loadDashboard();
});
</script>

<template>
  <PageShell size="dashboard">
    <PageHeader
      eyebrow="Live operations"
      title="Dashboard"
      :subtitle="`Daily workshop performance for ${displayedDay}.`"
      :icon="Gauge"
      tone="dashboard"
    >
      <template #actions>
        <label class="date-control">
          <CalendarDays :size="18" />
          <input
            v-model="selectedDay"
            type="date"
            :disabled="isLoading"
            aria-label="Dashboard day"
            @change="handleDateChange"
          />
        </label>

        <ActionButton variant="secondary" :disabled="isLoading" @click="loadDashboard">
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
          <span>Refresh</span>
        </ActionButton>
      </template>
    </PageHeader>

    <LoadingState v-if="isLoading && !hasLoaded" message="Loading dashboard..." />

    <template v-else>
      <section class="hero-metrics" aria-label="Dashboard headline metrics">
        <article
          v-for="metric in heroMetrics"
          :key="metric.label"
          class="hero-metric"
          :class="`hero-metric--${metric.tone}`"
        >
          <span class="hero-metric__icon">
            <component :is="metric.icon" :size="22" />
          </span>
          <span class="hero-metric__label">{{ metric.label }}</span>
          <strong>{{ metric.value }}</strong>
          <small>{{ metric.helper }}</small>
        </article>
      </section>

      <section class="dashboard-layout-grid">
        <ContentPanel class="dashboard-panel dashboard-panel--status">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Work orders</span>
              <h2>Order flow</h2>
            </div>
            <span class="panel-chip">{{ dashboard.totalOrders }} total</span>
          </div>

          <div v-if="hasOrders" class="status-list">
            <div
              v-for="item in statusItems"
              :key="item.label"
              class="status-row"
              :style="{ '--status-color': item.color }"
            >
              <span class="status-row__icon">
                <component :is="item.icon" :size="18" />
              </span>
              <div class="status-row__content">
                <div class="status-row__top">
                  <strong>{{ item.label }}</strong>
                  <span>{{ item.value }}</span>
                </div>
                <div class="status-track">
                  <span :style="{ width: getStatusWidth(item.value) }"></span>
                </div>
              </div>
            </div>
          </div>

          <div v-else class="zero-state">
            <Activity :size="28" />
            <strong>No work orders on this day</strong>
            <span>The dashboard is ready; it will light up once jobs start landing.</span>
          </div>
        </ContentPanel>

        <ContentPanel class="dashboard-panel">
          <div class="panel-heading">
            <div>
              <span class="panel-kicker">Money lane</span>
              <h2>Financial snapshot</h2>
            </div>
            <span class="panel-chip">{{ formatMoney(dashboard.netProfit) }} net</span>
          </div>

          <div class="money-stack">
            <div
              v-for="item in moneyBreakdown"
              :key="item.label"
              class="money-row"
            >
              <span>
                <component :is="item.icon" :size="17" />
                {{ item.label }}
              </span>
              <strong>{{ item.formatted }}</strong>
            </div>
          </div>

          <div class="profit-strip" :class="{ 'profit-strip--loss': dashboard.netProfit < 0 }">
            <span>Net profit</span>
            <strong>{{ formatMoney(dashboard.netProfit) }}</strong>
          </div>
        </ContentPanel>
      </section>

      <section class="ratio-grid" aria-label="Dashboard ratios">
        <article
          v-for="item in ratioCards"
          :key="item.label"
          class="ratio-card"
          :style="{ '--ratio-progress': getRatioProgress(item.value), '--ratio-accent': item.accent }"
        >
          <span class="ratio-card__dial">
            <component :is="item.icon" :size="18" />
          </span>
          <strong>{{ formatPercent(item.value) }}</strong>
          <small>{{ item.label }}</small>
        </article>
      </section>

      <section class="insight-grid" aria-label="Dashboard insights">
        <article class="insight-card">
          <span class="insight-card__icon">
            <UserRound :size="20" />
          </span>
          <div>
            <span>Unique customers</span>
            <strong>{{ dashboard.uniqueCustomers }}</strong>
          </div>
        </article>

        <article class="insight-card">
          <span class="insight-card__icon">
            <Car :size="20" />
          </span>
          <div>
            <span>Unique vehicles</span>
            <strong>{{ dashboard.uniqueVehicles }}</strong>
          </div>
        </article>

        <article class="insight-card">
          <span class="insight-card__icon">
            <Percent :size="20" />
          </span>
          <div>
            <span>Orders per vehicle</span>
            <strong>{{ formatPercent(dashboard.ordersPerVehicle) }}</strong>
          </div>
        </article>
      </section>

      <section class="module-grid" aria-label="Dashboard modules">
        <RouterLink
          v-for="module in moduleCards"
          :key="module.title"
          class="module-card"
          :class="`module-card--${module.tone}`"
          :to="{ name: module.routeName }"
        >
          <span class="module-icon">
            <component :is="module.icon" :size="23" />
          </span>
          <span class="module-copy">
            <strong>{{ module.title }}</strong>
            <small>{{ module.text }}</small>
          </span>
          <ArrowRight class="module-arrow" :size="18" />
        </RouterLink>
      </section>
    </template>
  </PageShell>
</template>

<style scoped>
.date-control {
  display: inline-flex;
  align-items: center;
  gap: 9px;
  min-height: 44px;
  padding: 0 12px;
  color: #f8fafc;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius: 8px;
  font-size: 14px;
  font-weight: 850;
}

.date-control input {
  min-width: 142px;
  color: #fff;
  background: transparent;
  border: 0;
  outline: none;
}

.date-control input::-webkit-calendar-picker-indicator {
  filter: invert(1);
  opacity: 0.84;
}

.hero-metrics {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 16px;
}

.hero-metric {
  position: relative;
  overflow: hidden;
  display: grid;
  gap: 8px;
  min-height: 162px;
  padding: 18px;
  background: #fff;
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 16px 34px rgba(15, 23, 42, 0.08);
}

.hero-metric::after {
  position: absolute;
  right: -28px;
  bottom: -42px;
  width: 118px;
  height: 118px;
  content: "";
  background:
    repeating-conic-gradient(from 6deg, rgba(15, 23, 42, 0.08) 0 12deg, transparent 12deg 24deg),
    radial-gradient(circle, transparent 38%, rgba(15, 23, 42, 0.07) 39% 46%, transparent 47%);
  border-radius: 999px;
}

.hero-metric--orders {
  --metric-accent: #38bdf8;
}

.hero-metric--revenue {
  --metric-accent: #f59e0b;
}

.hero-metric--profit {
  --metric-accent: #22c55e;
}

.hero-metric--loss {
  --metric-accent: #ef4444;
}

.hero-metric__icon {
  display: grid;
  place-items: center;
  width: 46px;
  height: 46px;
  color: color-mix(in srgb, var(--metric-accent) 74%, #111827);
  background: color-mix(in srgb, var(--metric-accent) 14%, #fff);
  border: 1px solid color-mix(in srgb, var(--metric-accent) 26%, transparent);
  border-radius: 8px;
}

.hero-metric__label {
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.hero-metric strong {
  position: relative;
  z-index: 1;
  color: #111827;
  font-size: 34px;
  line-height: 1;
}

.hero-metric small {
  position: relative;
  z-index: 1;
  color: #64748b;
  font-size: 13px;
  font-weight: 750;
}

.dashboard-layout-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(320px, 0.8fr);
  gap: 16px;
  margin-bottom: 16px;
}

.dashboard-panel {
  padding: 22px;
}

.panel-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 20px;
}

.panel-kicker {
  display: block;
  margin-bottom: 6px;
  color: #b45309;
  font-size: 11px;
  font-weight: 900;
  text-transform: uppercase;
}

.panel-heading h2 {
  margin: 0;
  color: #111827;
  font-size: 22px;
  letter-spacing: 0;
}

.panel-chip {
  display: inline-flex;
  align-items: center;
  min-height: 32px;
  padding: 0 10px;
  color: #334155;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}

.status-list,
.money-stack {
  display: grid;
  gap: 12px;
}

.status-row {
  display: grid;
  grid-template-columns: 44px minmax(0, 1fr);
  gap: 12px;
  align-items: center;
  padding: 12px;
  background: #f8fafc;
  border: 1px solid #eef2f7;
  border-radius: 8px;
}

.status-row__icon {
  display: grid;
  place-items: center;
  width: 44px;
  height: 44px;
  color: var(--status-color);
  background: color-mix(in srgb, var(--status-color) 13%, #fff);
  border-radius: 8px;
}

.status-row__content {
  min-width: 0;
}

.status-row__top {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 8px;
  color: #111827;
  font-size: 14px;
  font-weight: 900;
}

.status-track {
  overflow: hidden;
  height: 8px;
  background: #e5e7eb;
  border-radius: 999px;
}

.status-track span {
  display: block;
  height: 100%;
  background: var(--status-color);
  border-radius: inherit;
  transition: width 220ms ease;
}

.zero-state {
  display: grid;
  place-items: center;
  min-height: 248px;
  padding: 28px;
  color: #64748b;
  text-align: center;
  background:
    linear-gradient(135deg, rgba(34, 197, 94, 0.08), transparent 42%),
    #f8fafc;
  border: 1px dashed #cbd5e1;
  border-radius: 8px;
}

.zero-state strong {
  color: #111827;
  font-size: 18px;
}

.zero-state span {
  max-width: 310px;
  font-size: 14px;
  line-height: 1.5;
}

.money-row,
.profit-strip {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  padding: 13px 0;
  border-bottom: 1px solid #eef2f7;
}

.money-row span,
.profit-strip span {
  display: inline-flex;
  align-items: center;
  gap: 9px;
  color: #64748b;
  font-size: 13px;
  font-weight: 850;
}

.money-row strong,
.profit-strip strong {
  color: #111827;
  font-size: 17px;
}

.profit-strip {
  margin-top: 18px;
  padding: 16px;
  color: #064e3b;
  background: #dcfce7;
  border: 0;
  border-radius: 8px;
}

.profit-strip--loss {
  color: #7f1d1d;
  background: #fee2e2;
}

.ratio-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 16px;
}

.ratio-card {
  display: grid;
  justify-items: center;
  gap: 7px;
  padding: 18px;
  text-align: center;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 12px 28px rgba(15, 23, 42, 0.06);
}

.ratio-card__dial {
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  color: #111827;
  background:
    radial-gradient(circle at center, #fff 0 57%, transparent 58%),
    conic-gradient(var(--ratio-accent) var(--ratio-progress), #e5e7eb 0);
  border-radius: 999px;
}

.ratio-card strong {
  color: #111827;
  font-size: 23px;
  line-height: 1;
}

.ratio-card small {
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.insight-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 16px;
}

.insight-card {
  display: flex;
  align-items: center;
  gap: 13px;
  padding: 16px;
  background: #fff;
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.06);
}

.insight-card__icon {
  display: grid;
  place-items: center;
  width: 44px;
  height: 44px;
  color: #0f766e;
  background: #ccfbf1;
  border-radius: 8px;
}

.insight-card span {
  color: #64748b;
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.insight-card strong {
  display: block;
  margin-top: 4px;
  color: #111827;
  font-size: 24px;
}

.module-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
}

.module-card {
  position: relative;
  display: grid;
  grid-template-columns: 48px minmax(0, 1fr) 28px;
  align-items: center;
  gap: 13px;
  min-height: 112px;
  padding: 18px;
  color: inherit;
  text-decoration: none;
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 16px 34px rgba(15, 23, 42, 0.08);
  transition:
    transform 160ms ease,
    box-shadow 160ms ease,
    border-color 160ms ease;
}

.module-card--inventory {
  --module-accent: #f59e0b;
}

.module-card--orders {
  --module-accent: #8b5cf6;
}

.module-card--schedule {
  --module-accent: #22c55e;
}

.module-card--billing {
  --module-accent: #0f766e;
}

.module-card--people {
  --module-accent: #38bdf8;
}

.module-card--customers {
  --module-accent: #14b8a6;
}

.module-card--service {
  --module-accent: #ef4444;
}

.module-card:hover {
  transform: translateY(-2px);
  border-color: color-mix(in srgb, var(--module-accent) 40%, transparent);
  box-shadow: 0 24px 48px rgba(15, 23, 42, 0.12);
}

.module-icon {
  display: grid;
  place-items: center;
  width: 48px;
  height: 48px;
  color: color-mix(in srgb, var(--module-accent) 78%, #111827);
  background: color-mix(in srgb, var(--module-accent) 15%, #fff);
  border-radius: 8px;
}

.module-copy strong,
.module-copy small {
  display: block;
}

.module-copy strong {
  color: #111827;
  font-size: 18px;
}

.module-copy small {
  margin-top: 6px;
  color: #64748b;
  font-size: 13px;
  line-height: 1.45;
}

.module-arrow {
  color: #94a3b8;
  transition:
    color 160ms ease,
    transform 160ms ease;
}

.module-card:hover .module-arrow {
  color: var(--module-accent);
  transform: translateX(3px);
}

@media (max-width: 980px) {
  .hero-metrics,
  .dashboard-layout-grid,
  .ratio-grid,
  .insight-grid,
  .module-grid {
    grid-template-columns: 1fr;
  }
}
</style>
