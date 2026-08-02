<script setup>
import { Car, Mail, Pencil, Phone, Plus, RefreshCw, Trash2, UserRound } from "@lucide/vue";
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
import { deleteCustomer, getCustomers } from "@/services/customers.service";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();
const customers = ref([]);
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingCustomerId = ref(null);

const hasCustomers = computed(() => customers.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => {
  return Math.min(page.value * pageSize.value, totalItems.value);
});
const vehicleCount = computed(() => {
  return customers.value.reduce((count, customer) => count + getVehicles(customer).length, 0);
});

function getValue(source, camelKey, pascalKey, fallback = "") {
  return source?.[camelKey] ?? source?.[pascalKey] ?? fallback;
}

function getCustomerId(customer) {
  return getValue(customer, "id", "Id");
}

function getFirstName(customer) {
  return getValue(customer, "firstName", "FirstName");
}

function getLastName(customer) {
  return getValue(customer, "lastName", "LastName");
}

function getFullName(customer) {
  const fullName = getValue(customer, "fullName", "FullName");

  if (fullName) {
    return fullName;
  }

  return `${getFirstName(customer)} ${getLastName(customer)}`.trim() || "Unnamed customer";
}

function getVehicles(customer) {
  return getValue(customer, "vehicles", "Vehicles", []);
}

function getErrorMessage(error) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.title ||
    "Something went wrong while loading customers."
  );
}

async function loadCustomers(targetPage = page.value) {
  isLoading.value = true;

  try {
    const { data } = await getCustomers({
      Page: targetPage,
      PageSize: pageSize.value,
    });

    customers.value = data.items ?? data.Items ?? [];
    page.value = data.page ?? data.Page ?? targetPage;
    pageSize.value = data.pageSize ?? data.PageSize ?? pageSize.value;
    totalItems.value = data.totalItems ?? data.TotalItems ?? customers.value.length;
    totalPages.value = data.totalPages ?? data.TotalPages ?? 1;
  } catch (error) {
    ui.showErrorToast(getErrorMessage(error), "Unable to load customers");
  } finally {
    isLoading.value = false;
  }
}

async function handleDelete(customer) {
  const customerId = getCustomerId(customer);
  const customerName = getFullName(customer);

  if (!customerId) return;

  const shouldDelete = await ui.confirm({
    title: "Delete customer?",
    message: `This will permanently remove "${customerName}" and their vehicle records.`,
    confirmText: "Delete customer",
    cancelText: "Keep customer",
    variant: "danger",
  });

  if (!shouldDelete) return;

  deletingCustomerId.value = customerId;

  try {
    await deleteCustomer(customerId);

    ui.showSuccessToast(`"${customerName}" was removed from customers.`, "Customer deleted");

    if (customers.value.length === 1 && page.value > 1) {
      await loadCustomers(page.value - 1);
    } else {
      await loadCustomers(page.value);
    }
  } catch (error) {
    ui.showErrorToast(
      error.response?.data?.detail ||
        error.response?.data?.title ||
        "Unable to delete this customer.",
      "Delete failed"
    );
  } finally {
    deletingCustomerId.value = null;
  }
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadCustomers(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadCustomers(page.value + 1);
}

onMounted(() => {
  loadCustomers();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="Customers"
      title="Customers"
      subtitle="Manage customer records and the vehicles tied to each job."
      :icon="UserRound"
      tone="customers"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh customers"
          @click="loadCustomers()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton :to="{ name: 'customer-create' }">
          <Plus :size="18" />
          <span>New customer</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Customers summary">
      <SummaryCard label="Total customers" :value="totalItems" />
      <SummaryCard label="Vehicles on page" :value="vehicleCount" />
      <SummaryCard label="Current page" :value="`${page} / ${totalPages}`" />
    </SummaryGrid>

    <ContentPanel>
      <LoadingState v-if="isLoading && !hasCustomers" message="Loading customers..." />

      <EmptyState
        v-else-if="!hasCustomers"
        title="No customers yet"
        message="Create the first customer record and attach at least one vehicle."
      >
        <template #icon>
          <UserRound :size="28" />
        </template>
        <template #action>
          <ActionButton :to="{ name: 'customer-create' }">
            <Plus :size="18" />
            <span>Create customer</span>
          </ActionButton>
        </template>
      </EmptyState>

      <div v-else class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Contact</th>
              <th>Vehicles</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="customer in customers" :key="getCustomerId(customer)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar">
                    <UserRound :size="17" />
                  </span>
                  <div>
                    <strong>{{ getFullName(customer) }}</strong>
                    <small>{{ getCustomerId(customer) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <div class="contact-stack">
                  <span>
                    <Phone :size="14" />
                    {{ getValue(customer, "phone", "Phone") }}
                  </span>
                  <span>
                    <Mail :size="14" />
                    {{ getValue(customer, "email", "Email") }}
                  </span>
                </div>
              </td>
              <td>
                <span class="vehicle-pill">
                  <Car :size="14" />
                  {{ getVehicles(customer).length }} vehicles
                </span>
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'customer-edit', params: { id: getCustomerId(customer) } }"
                    aria-label="Edit customer"
                  >
                    <Pencil :size="16" />
                  </ActionButton>
                  <ActionButton
                    variant="danger"
                    size="sm"
                    icon-only
                    :disabled="deletingCustomerId === getCustomerId(customer)"
                    aria-label="Delete customer"
                    @click="handleDelete(customer)"
                  >
                    <Trash2
                      v-if="deletingCustomerId !== getCustomerId(customer)"
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
        v-if="hasCustomers"
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
.contact-stack {
  display: grid;
  gap: 6px;
  color: #6b7280;
  font-size: 13px;
  font-weight: 750;
}

.contact-stack span,
.vehicle-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.vehicle-pill {
  justify-content: center;
  min-width: 112px;
  padding: 7px 10px;
  color: #075985;
  background: #e0f2fe;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}
</style>
