<script setup>
import { ArrowLeft, LoaderCircle, Save, UserPlus } from "@lucide/vue";
import { computed, reactive, ref } from "vue";
import { useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormField from "@/components/Shared/FormField.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { addEmployee } from "@/services/employees.service";
import { useUiStore } from "@/store/modules/ui";

const router = useRouter();
const ui = useUiStore();

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

const canSubmit = computed(() => {
  return !isLoading.value && form.firstName.trim() && form.lastName.trim() && form.role;
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

function getBackendErrorMessage(error) {
  const data = error.response?.data;

  if (!data) {
    return "Unable to create employee. Please check your connection and try again.";
  }

  if (typeof data === "string") {
    return data;
  }

  if (data.detail) {
    return data.detail;
  }

  if (data.title) {
    return data.title;
  }

  if (data.errors) {
    const messages = Object.values(data.errors).flat();
    if (messages.length) {
      return messages.join(" ");
    }
  }

  return "Unable to create employee. Please try again.";
}

async function handleSubmit() {
  if (!validateForm()) return;

  isLoading.value = true;

  try {
    const payload = {
      firstName: form.firstName.trim(),
      lastName: form.lastName.trim(),
      role: Number(form.role),
    };

    await addEmployee(payload);

    ui.showSuccessToast(
      `Employee "${payload.firstName} ${payload.lastName}" created as ${getRoleLabel(payload.role)}.`,
      "Employee created"
    );
    await router.push({ name: "employees" });
  } catch (error) {
    ui.showErrorToast(getBackendErrorMessage(error), "Create employee failed");
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="People"
      title="Create employee"
      subtitle="Add a manager or technician to the workshop team."
      :icon="UserPlus"
      tone="people"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'employees' }">
          <ArrowLeft :size="18" />
          <span>Back to employees</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Employee details" subtitle="First name, last name, and role are required.">
      <template #icon>
        <UserPlus :size="26" />
      </template>

      <form class="crud-form" novalidate @submit.prevent="handleSubmit">
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
          <LoaderCircle v-if="isLoading" class="spinning" :size="18" />
          <Save v-else :size="18" />
          <span>{{ isLoading ? "Creating employee..." : "Create employee" }}</span>
        </ActionButton>
      </form>
    </FormPanel>
  </PageShell>
</template>
