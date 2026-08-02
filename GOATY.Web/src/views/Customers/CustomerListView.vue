<script setup>
import { Car, Mail, Pencil, Phone, Plus, RefreshCw, Trash2, UserRound, X } from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import EntityFilterBar from "@/components/Shared/EntityFilterBar.vue";
import EntityListShell from "@/components/Shared/EntityListShell.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import StatusChip from "@/components/Shared/StatusChip.vue";
import SummaryCard from "@/components/Shared/SummaryCard.vue";
import SummaryGrid from "@/components/Shared/SummaryGrid.vue";
import { deleteCustomer, getCustomers } from "@/services/customers.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { asArray, matchesSearch, readValue } from "@/utils/objectAccess";

const ui = useUiStore();
const customers = ref([]);
const search = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingCustomerId = ref(null);
const loadErrorMessage = ref("");

const visibleCustomers = computed(() => {
  return customers.value.filter((customer) => {
    const vehicles = getVehicles(customer);

    return matchesSearch(
      [
        getCustomerId(customer),
        getFullName(customer),
        getValue(customer, "phone", "Phone"),
        getValue(customer, "email", "Email"),
        ...vehicles.flatMap((vehicle) => [
          getValue(vehicle, "brand", "Brand"),
          getValue(vehicle, "model", "Model"),
          getValue(vehicle, "year", "Year"),
          getValue(vehicle, "licensePlate", "LicensePlate"),
        ]),
      ],
      search.value
    );
  });
});

const hasVisibleCustomers = computed(() => visibleCustomers.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
const vehicleCount = computed(() => {
  return customers.value.reduce((count, customer) => count + getVehicles(customer).length, 0);
});
const resultLabel = computed(() => {
  if (search.value.trim()) {
    return `${visibleCustomers.value.length} match${visibleCustomers.value.length === 1 ? "" : "es"} on this page`;
  }

  return `${totalItems.value} total`;
});
const emptyTitle = computed(() =>
  search.value.trim() ? "No customers match this search" : "No customers yet"
);
const emptyMessage = computed(() =>
  search.value.trim()
    ? "Clear the search or move to another page to keep scanning customers."
    : "Create the first customer record and attach at least one vehicle."
);

function getValue(source, camelKey, pascalKey, fallback = "") {
  return readValue(source, camelKey, pascalKey, fallback);
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
  return asArray(getValue(customer, "vehicles", "Vehicles", []));
}

async function loadCustomers(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getCustomers({
      Page: targetPage,
      PageSize: pageSize.value,
    });
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    customers.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading customers."
    );
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
      getBackendErrorMessage(error, "Unable to delete this customer."),
      "Delete failed"
    );
  } finally {
    deletingCustomerId.value = null;
  }
}

function clearSearch() {
  search.value = "";
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

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasVisibleCustomers"
      :error-message="loadErrorMessage"
      loading-message="Loading customers..."
      :empty-title="emptyTitle"
      :empty-message="emptyMessage"
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadCustomers()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="search"
          search-placeholder="Search current page by name, phone, email, or vehicle"
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
        <UserRound :size="28" />
      </template>

      <template #emptyAction>
        <ActionButton v-if="search" variant="secondary" @click="clearSearch">
          <X :size="17" />
          <span>Clear search</span>
        </ActionButton>
        <ActionButton v-else :to="{ name: 'customer-create' }">
          <Plus :size="18" />
          <span>Create customer</span>
        </ActionButton>
      </template>

      <div class="table-wrap">
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
            <tr v-for="customer in visibleCustomers" :key="getCustomerId(customer)">
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
                <StatusChip
                  :label="`${getVehicles(customer).length} vehicles`"
                  tone="customers"
                  :icon="Car"
                />
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
    </EntityListShell>
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

.contact-stack span {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
</style>
