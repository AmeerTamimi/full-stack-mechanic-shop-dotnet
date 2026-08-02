<script setup>
import { Pencil, Plus, RefreshCw, ShieldCheck, Trash2, UserCog, Users } from "@lucide/vue";
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
import { deleteEmployee, getEmployees } from "@/services/employees.service";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();
const employees = ref([]);
const page = ref(1);
const pageSize = ref(10);
const totalItems = ref(0);
const totalPages = ref(1);
const isLoading = ref(false);
const deletingEmployeeId = ref(null);

const hasEmployees = computed(() => employees.value.length > 0);
const firstItemNumber = computed(() => {
  if (!totalItems.value) return 0;
  return (page.value - 1) * pageSize.value + 1;
});
const lastItemNumber = computed(() => {
  return Math.min(page.value * pageSize.value, totalItems.value);
});
const managerCount = computed(() => {
  return employees.value.filter((employee) => Number(getRoleValue(employee)) === 1).length;
});
const technicianCount = computed(() => {
  return employees.value.filter((employee) => Number(getRoleValue(employee)) === 2).length;
});

function getRoleValue(employee) {
  return employee.role ?? employee.Role;
}

function getEmployeeId(employee) {
  return employee.id ?? employee.Id;
}

function getFirstName(employee) {
  return employee.firstName ?? employee.FirstName ?? "";
}

function getLastName(employee) {
  return employee.lastName ?? employee.LastName ?? "";
}

function getFullName(employee) {
  const fullName = employee.fullName ?? employee.FullName;

  if (fullName) {
    return fullName;
  }

  return `${getFirstName(employee)} ${getLastName(employee)}`.trim() || "Unnamed employee";
}

function getEmail(employee) {
  return employee.email ?? employee.Email ?? "";
}

function getRoleLabel(role) {
  const roleValue = Number(role);

  if (roleValue === 1) return "Manager";
  if (roleValue === 2) return "Technician";

  return "Unknown role";
}

function getErrorMessage(error) {
  return (
    error.response?.data?.detail ||
    error.response?.data?.title ||
    "Something went wrong while loading employees."
  );
}

async function loadEmployees(targetPage = page.value) {
  isLoading.value = true;

  try {
    const { data } = await getEmployees({
      Page: targetPage,
      PageSize: pageSize.value,
    });

    employees.value = data.items ?? data.Items ?? [];
    page.value = data.page ?? data.Page ?? targetPage;
    pageSize.value = data.pageSize ?? data.PageSize ?? pageSize.value;
    totalItems.value = data.totalItems ?? data.TotalItems ?? employees.value.length;
    totalPages.value = data.totalPages ?? data.TotalPages ?? 1;
  } catch (error) {
    ui.showErrorToast(getErrorMessage(error), "Unable to load employees");
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
      error.response?.data?.detail ||
        error.response?.data?.title ||
        "Unable to delete this employee.",
      "Delete failed"
    );
  } finally {
    deletingEmployeeId.value = null;
  }
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

    <ContentPanel>
      <LoadingState v-if="isLoading && !hasEmployees" message="Loading employees..." />

      <EmptyState
        v-else-if="!hasEmployees"
        title="No employees yet"
        message="Add your first manager or technician to start building the team."
      >
        <template #icon>
          <Users :size="28" />
        </template>
        <template #action>
          <ActionButton :to="{ name: 'employee-create' }">
            <Plus :size="18" />
            <span>Create employee</span>
          </ActionButton>
        </template>
      </EmptyState>

      <div v-else class="table-wrap">
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
            <tr v-for="employee in employees" :key="getEmployeeId(employee)">
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
                <span
                  class="role-pill"
                  :class="{ 'role-pill--manager': Number(getRoleValue(employee)) === 1 }"
                >
                  <ShieldCheck v-if="Number(getRoleValue(employee)) === 1" :size="14" />
                  <UserCog v-else :size="14" />
                  {{ getRoleLabel(getRoleValue(employee)) }}
                </span>
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

      <PaginationBar
        v-if="hasEmployees"
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
.role-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  min-width: 118px;
  padding: 7px 10px;
  color: #075985;
  background: #e0f2fe;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 900;
  white-space: nowrap;
}

.role-pill--manager {
  color: #92400e;
  background: #fef3c7;
}
</style>
