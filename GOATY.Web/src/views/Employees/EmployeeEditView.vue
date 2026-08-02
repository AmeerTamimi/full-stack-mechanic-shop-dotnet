<script setup>
import { ArrowLeft, LoaderCircle, Pencil, Save } from "@lucide/vue";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getEmployee, updateEmployee } from "@/services/employees.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage } from "@/utils/api";
import { readValue } from "@/utils/objectAccess";

const route = useRoute();
const router = useRouter();
const ui = useUiStore();

const employeeId = computed(() => route.params.id?.toString());

const roleOptions = [
  { value: "1", label: "Manager" },
  { value: "2", label: "Technician" },
];

const form = reactive({
  firstName: "",
  lastName: "",
  role: "",
});

const errors = reactive({
  firstName: "",
  lastName: "",
  role: "",
});

const isLoading = ref(false);
const isSaving = ref(false);

const canSubmit = computed(() => {
  return (
    !isLoading.value &&
    !isSaving.value &&
    form.firstName.trim() &&
    form.lastName.trim() &&
    form.role
  );
});

function clearFieldErrors() {
  errors.firstName = "";
  errors.lastName = "";
  errors.role = "";
}

function validateName(value, label) {
  const trimmedValue = value.trim();

  if (trimmedValue.length < 2) {
    return `${label} must be at least 2 characters.`;
  }

  if (trimmedValue.length > 50) {
    return `${label} must be 50 characters or less.`;
  }

  return "";
}

function validateForm() {
  clearFieldErrors();

  errors.firstName = validateName(form.firstName, "First name");
  errors.lastName = validateName(form.lastName, "Last name");

  if (!roleOptions.some((role) => role.value === form.role)) {
    errors.role = "Choose a valid employee role.";
  }

  return !errors.firstName && !errors.lastName && !errors.role;
}

function getRoleLabel(roleValue) {
  return roleOptions.find((role) => role.value === String(roleValue))?.label ?? "Unknown role";
}

function fillForm(employee) {
  form.firstName = readValue(employee, "firstName", "FirstName");
  form.lastName = readValue(employee, "lastName", "LastName");
  form.role = String(readValue(employee, "role", "Role"));
}

async function loadEmployee() {
  if (!employeeId.value) {
    ui.showErrorToast("Missing employee id.", "Unable to load employee");
    await router.push({ name: "employees" });
    return;
  }

  isLoading.value = true;

  try {
    const { data } = await getEmployee(employeeId.value);
    fillForm(data);
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load this employee. They may have been deleted or you may not have access."
      ),
      "Unable to load employee"
    );
    await router.push({ name: "employees" });
  } finally {
    isLoading.value = false;
  }
}

async function handleSubmit() {
  if (!validateForm()) return;

  isSaving.value = true;

  try {
    const payload = {
      firstName: form.firstName.trim(),
      lastName: form.lastName.trim(),
      role: Number(form.role),
    };

    await updateEmployee(employeeId.value, payload);

    ui.showSuccessToast(
      `Employee "${payload.firstName} ${payload.lastName}" updated as ${getRoleLabel(payload.role)}.`,
      "Employee updated"
    );
    await router.push({ name: "employees" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update employee. Please try again."),
      "Update employee failed"
    );
  } finally {
    isSaving.value = false;
  }
}

onMounted(() => {
  loadEmployee();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="People"
      title="Edit employee"
      subtitle="Update employee name and workshop role."
      :icon="Pencil"
      tone="people"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'employees' }">
          <ArrowLeft :size="18" />
          <span>Back to employees</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Employee details" subtitle="Changes will be saved to the workshop team record.">
      <template #icon>
        <Pencil :size="25" />
      </template>

      <LoadingState v-if="isLoading" message="Loading employee..." />

      <form v-else class="crud-form" novalidate @submit.prevent="handleSubmit">
        <div class="field-grid">
          <FormField
            id="employee-first-name"
            label="First name"
            :error="errors.firstName"
          >
            <input
              id="employee-first-name"
              v-model="form.firstName"
              type="text"
              maxlength="50"
              placeholder="Ameer"
              :aria-invalid="Boolean(errors.firstName)"
              aria-describedby="employee-first-name-error"
              @blur="validateForm"
            />
          </FormField>

          <FormField id="employee-last-name" label="Last name" :error="errors.lastName">
            <input
              id="employee-last-name"
              v-model="form.lastName"
              type="text"
              maxlength="50"
              placeholder="Tamimi"
              :aria-invalid="Boolean(errors.lastName)"
              aria-describedby="employee-last-name-error"
              @blur="validateForm"
            />
          </FormField>
        </div>

        <FormField id="employee-role" label="Role" :error="errors.role">
          <select
            id="employee-role"
            v-model="form.role"
            :aria-invalid="Boolean(errors.role)"
            aria-describedby="employee-role-error"
            @blur="validateForm"
          >
            <option value="" disabled>Select role</option>
            <option v-for="role in roleOptions" :key="role.value" :value="role.value">
              {{ role.label }}
            </option>
          </select>
        </FormField>

        <ActionButton type="submit" size="lg" block :disabled="!canSubmit">
          <LoaderCircle v-if="isSaving" class="spinning" :size="18" />
          <Save v-else :size="18" />
          <span>{{ isSaving ? "Saving changes..." : "Save changes" }}</span>
        </ActionButton>
      </form>
    </FormPanel>
  </PageShell>
</template>
