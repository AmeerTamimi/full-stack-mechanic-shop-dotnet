<script setup>
import { ArrowLeft, Pencil } from "@lucide/vue";
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import CustomerForm from "@/components/Customers/CustomerForm.vue";
import ActionButton from "@/components/Shared/ActionButton.vue";
import FormPanel from "@/components/Shared/FormPanel.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PageHeader from "@/components/Shared/PageHeader.vue";
import PageShell from "@/components/Shared/PageShell.vue";
import { getCustomer, updateCustomer } from "@/services/customers.service";
import { useUiStore } from "@/store/modules/ui";
import { getBackendErrorMessage } from "@/utils/api";

const route = useRoute();
const router = useRouter();
const ui = useUiStore();

const customerId = computed(() => route.params.id?.toString());
const customer = ref(null);
const isLoading = ref(false);
const isSaving = ref(false);

async function loadCustomer() {
  if (!customerId.value) {
    ui.showErrorToast("Missing customer id.", "Unable to load customer");
    await router.push({ name: "customers" });
    return;
  }

  isLoading.value = true;

  try {
    const { data } = await getCustomer(customerId.value);
    customer.value = data;
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(
        error,
        "Unable to load this customer. They may have been deleted or you may not have access."
      ),
      "Unable to load customer"
    );
    await router.push({ name: "customers" });
  } finally {
    isLoading.value = false;
  }
}

async function handleSubmit(payload) {
  isSaving.value = true;

  try {
    await updateCustomer(customerId.value, payload);

    ui.showSuccessToast(
      `Customer "${payload.firstName} ${payload.lastName}" updated with ${payload.vehicles.length} vehicle(s).`,
      "Customer updated"
    );
    await router.push({ name: "customers" });
  } catch (error) {
    ui.showErrorToast(
      getBackendErrorMessage(error, "Unable to update customer. Please try again."),
      "Update customer failed"
    );
  } finally {
    isSaving.value = false;
  }
}

onMounted(() => {
  loadCustomer();
});
</script>

<template>
  <PageShell size="form">
    <PageHeader
      eyebrow="Customers"
      title="Edit customer"
      subtitle="Update customer contact information and vehicle records."
      :icon="Pencil"
      tone="customers"
    >
      <template #actions>
        <ActionButton variant="secondary" :to="{ name: 'customers' }">
          <ArrowLeft :size="18" />
          <span>Back to customers</span>
        </ActionButton>
      </template>
    </PageHeader>

    <FormPanel title="Customer details" subtitle="Changes will be saved to the customer profile.">
      <template #icon>
        <Pencil :size="25" />
      </template>

      <LoadingState v-if="isLoading" message="Loading customer..." />

      <CustomerForm
        v-else
        :initial-customer="customer"
        :is-submitting="isSaving"
        submit-label="Save changes"
        submitting-label="Saving changes..."
        @submit="handleSubmit"
      />
    </FormPanel>
  </PageShell>
</template>
