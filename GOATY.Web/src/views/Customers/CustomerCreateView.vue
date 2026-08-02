<script setup>
import { ArrowLeft, UserPlus } from "@lucide/vue";
import { ref } from "vue";
import { useRouter } from "vue-router";
import CustomerForm from "@/components/Customers/CustomerForm.vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { addCustomer } from "@/services/customers.service";
import { useUiStore } from "@/store/modules/ui";

const router = useRouter();
const ui = useUiStore();
const isSaving = ref(false);

function getBackendErrorMessage(error) {
  const data = error.response?.data;

  if (!data) {
    return "Unable to create customer. Please check your connection and try again.";
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

  return "Unable to create customer. Please try again.";
}

async function handleSubmit(payload) {
  isSaving.value = true;

  try {
    await addCustomer(payload);

    ui.showSuccessToast(
      `Customer "${payload.firstName} ${payload.lastName}" created with ${payload.vehicles.length} vehicle(s).`,
      "Customer created"
    );
    await router.push({ name: "customers" });
  } catch (error) {
    ui.showErrorToast(getBackendErrorMessage(error), "Create customer failed");
  } finally {
    isSaving.value = false;
  }
}
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Customers"
      title="Create customer"
      subtitle="Add a customer profile and attach their vehicle details."
      :icon="UserPlus"
      tone="customers"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'customers' }">
          <ArrowLeft :size="18" />
          <span>Back to customers</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Customer details" subtitle="Customer contact and at least one vehicle are required.">
      <template #icon>
        <UserPlus :size="26" />
      </template>

      <CustomerForm
        :is-submitting="isSaving"
        submit-label="Create customer"
        submitting-label="Creating customer..."
        @submit="handleSubmit"
      />
    </FormPanel>
  </PageShell>
</template>
