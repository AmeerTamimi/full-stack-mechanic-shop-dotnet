<script setup>
import {
  Pencil,
  Plus,
  RefreshCw,
  ShieldCheck,
  Trash2,
  UserCog,
  Users,
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
import { deleteEmployee, getEmployees } from "@/services/employees.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage, normalizePaginatedResponse } from "@/utils/api";
import { matchesSearch, readValue } from "@/utils/objectAccess";

const ui = useUiStore();
const employees = ref([]);
const search = ref("");
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingEmployeeId = ref(null);
const loadErrorMessage = ref("");

const visibleEmployees = computed(() => {
  return employees.value.filter((employee) =>
    matchesSearch(
      [
        getEmployeeId(employee),
        getFullName(employee),
        getEmail(employee),
        getRoleLabel(getRoleValue(employee)),
      ],
      search.value
    )
  );
});

const hasVisibleEmployees = computed(() => visibleEmployees.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => Math.min(page.value * pageSize.value, totalItems.value));
const managerCount = computed(() => {
  return employees.value.filter((employee) => Number(getRoleValue(employee)) === 1).length;
});
const technicianCount = computed(() => {
  return employees.value.filter((employee) => Number(getRoleValue(employee)) === 2).length;
});
const resultLabel = computed(() => {
  if (search.value.trim()) {
    return `${visibleEmployees.value.length} match${visibleEmployees.value.length === 1 ? "" : "es"} on this page`;
  }

  return `${totalItems.value} total`;
});
const emptyTitle = computed(() =>
  search.value.trim() ? "No employees match this search" : "No employees yet"
);
const emptyMessage = computed(() =>
  search.value.trim()
    ? "Clear the search or move to another page to keep scanning the team."
    : "Add your first manager or technician to start building the team."
);

function getRoleValue(employee) {
  return readValue(employee, "role", "Role");
}

function getEmployeeId(employee) {
  return readValue(employee, "id", "Id");
}

function getFirstName(employee) {
  return readValue(employee, "firstName", "FirstName");
}

function getLastName(employee) {
  return readValue(employee, "lastName", "LastName");
}

function getFullName(employee) {
  const fullName = readValue(employee, "fullName", "FullName");

  if (fullName) {
    return fullName;
  }

  return `${getFirstName(employee)} ${getLastName(employee)}`.trim() || "Unnamed employee";
}

function getEmail(employee) {
  return readValue(employee, "email", "Email");
}

function getRoleLabel(role) {
  const roleValue = Number(role);

  if (roleValue === 1) return "Manager";
  if (roleValue === 2) return "Technician";

  return "Unknown role";
}

function getRoleTone(role) {
  return Number(role) === 1 ? "manager" : "people";
}

async function loadEmployees(targetPage = page.value) {
  isLoading.value = true;
  loadErrorMessage.value = "";

  try {
    const { data } = await getEmployees({
      Page: targetPage,
      PageSize: pageSize.value,
    });
    const pagination = normalizePaginatedResponse(data, {
      page: targetPage,
      pageSize: pageSize.value,
    });

    employees.value = pagination.items;
    page.value = pagination.page;
    pageSize.value = pagination.pageSize;
    totalItems.value = pagination.totalItems;
    totalPages.value = pagination.totalPages;
  } catch (error) {
    loadErrorMessage.value = getBackendErrorMessage(
      error,
      "Something went wrong while loading employees."
    );
  } finally {
    isLoading.value = false;
  }
}

async function handleDelete(employee) {
  const employeeId = getEmployeeId(employee);
  const employeeName = getFullName(employee);

  if (!employeeId) return;

  const shouldDelete = await ui.confirm({
    title: "Delete employee?",
    message: `This will permanently remove "${employeeName}" from the workshop team.`,
    confirmText: "Delete employee",
    cancelText: "Keep employee",
    variant: "danger",
  });

  if (!shouldDelete) return;

  deletingEmployeeId.value = employeeId;

  try {
    await deleteEmployee(employeeId);

    ui.showSuccessToast(`"${employeeName}" was removed from the team.`, "Employee deleted");

    if (employees.value.length === 1 && page.value > 1) {
      await loadEmployees(page.value - 1);
    } else {
      await loadEmployees(page.value);
    }
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to delete this employee."),
      "Delete failed"
    );
  } finally {
    deletingEmployeeId.value = null;
  }
}

function clearSearch() {
  search.value = "";
}

function goToPreviousPage() {
  if (page.value <= 1 || isLoading.value) return;
  loadEmployees(page.value - 1);
}

function goToNextPage() {
  if (page.value >= totalPages.value || isLoading.value) return;
  loadEmployees(page.value + 1);
}

onMounted(() => {
  loadEmployees();
});
</script>

<template>
  <PageShell>
    <PageHeader
      eyebrow="People"
      title="Employees"
      subtitle="Manage the managers and technicians who keep the shop moving."
      :icon="Users"
      tone="people"
    >
      <template #actions>
        <ActionButton
          variant="secondary"
          icon-only
          :disabled="isLoading"
          aria-label="Refresh employees"
          @click="loadEmployees()"
        >
          <RefreshCw :class="{ spinning: isLoading }" :size="18" />
        </ActionButton>

        <ActionButton :to="{ name: 'employee-create' }">
          <Plus :size="18" />
          <span>New employee</span>
        </ActionButton>
      </template>
    </PageHeader>

    <SummaryGrid aria-label="Employees summary">
      <SummaryCard label="Total employees" :value="totalItems" />
      <SummaryCard label="Managers on page" :value="managerCount" />
      <SummaryCard label="Technicians on page" :value="technicianCount" />
    </SummaryGrid>

    <EntityListShell
      :is-loading="isLoading"
      :has-items="hasVisibleEmployees"
      :error-message="loadErrorMessage"
      loading-message="Loading employees..."
      :empty-title="emptyTitle"
      :empty-message="emptyMessage"
      :first-item="firstItemNumber"
      :last-item="lastItemNumber"
      :total-items="totalItems"
      :page="page"
      :total-pages="totalPages"
      @retry="loadEmployees()"
      @previous="goToPreviousPage"
      @next="goToNextPage"
    >
      <template #filters>
        <EntityFilterBar
          v-model:search="search"
          search-placeholder="Search current page by name, email, role, or ID"
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
        <Users :size="28" />
      </template>

      <template #emptyAction>
        <ActionButton v-if="search" variant="secondary" @click="clearSearch">
          <X :size="17" />
          <span>Clear search</span>
        </ActionButton>
        <ActionButton v-else :to="{ name: 'employee-create' }">
          <Plus :size="18" />
          <span>Create employee</span>
        </ActionButton>
      </template>

      <div class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Role</th>
              <th>Email</th>
              <th class="actions-column">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="employee in visibleEmployees" :key="getEmployeeId(employee)">
              <td>
                <div class="entity-name">
                  <span class="entity-avatar">
                    <UserCog :size="17" />
                  </span>
                  <div>
                    <strong>{{ getFullName(employee) }}</strong>
                    <small>{{ getEmployeeId(employee) }}</small>
                  </div>
                </div>
              </td>
              <td>
                <StatusChip
                  :label="getRoleLabel(getRoleValue(employee))"
                  :tone="getRoleTone(getRoleValue(employee))"
                  :icon="Number(getRoleValue(employee)) === 1 ? ShieldCheck : UserCog"
                />
              </td>
              <td>
                <span class="muted-text">{{ getEmail(employee) || "No email" }}</span>
              </td>
              <td>
                <div class="row-actions">
                  <ActionButton
                    variant="ghost"
                    size="sm"
                    icon-only
                    :to="{ name: 'employee-edit', params: { id: getEmployeeId(employee) } }"
                    aria-label="Edit employee"
                  >
                    <Pencil :size="16" />
                  </ActionButton>
                  <ActionButton
                    variant="danger"
                    size="sm"
                    icon-only
                    :disabled="deletingEmployeeId === getEmployeeId(employee)"
                    aria-label="Delete employee"
                    @click="handleDelete(employee)"
                  >
                    <Trash2
                      v-if="deletingEmployeeId !== getEmployeeId(employee)"
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
