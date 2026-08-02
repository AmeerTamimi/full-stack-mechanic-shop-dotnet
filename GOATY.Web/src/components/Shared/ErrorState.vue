<script setup>
import { AlertTriangle } from "@lucide/vue";
import ActionButton from "@/components/Shared/ActionButton.vue";

defineProps({
  title: {
    type: String,
    default: "Something went wrong",
  },
  message: {
    type: String,
    default: "Please try again.",
  },
  actionLabel: {
    type: String,
    default: "Try again",
  },
  showAction: {
    type: Boolean,
    default: true,
  },
});

defineEmits(["retry"]);
</script>

<template>
  <div class="error-state">
    <div class="error-state__icon">
      <slot name="icon">
        <AlertTriangle :size="28" />
      </slot>
    </div>

    <h2>{{ title }}</h2>
    <p>{{ message }}</p>

    <div v-if="showAction || $slots.action" class="error-state__action">
      <slot name="action">
        <ActionButton variant="secondary" @click="$emit('retry')">
          <span>{{ actionLabel }}</span>
        </ActionButton>
      </slot>
    </div>
  </div>
</template>

<style scoped>
.error-state {
  display: grid;
  justify-items: center;
  gap: 12px;
  padding: 64px 24px;
  text-align: center;
}

.error-state__icon {
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  color: #b91c1c;
  background: #fee2e2;
  border-radius: 8px;
}

.error-state h2 {
  margin: 0;
  max-width: 100%;
  overflow-wrap: anywhere;
  color: #111827;
  font-size: 24px;
}

.error-state p {
  max-width: 430px;
  margin: 0 0 8px;
  overflow-wrap: anywhere;
  color: #6b7280;
  line-height: 1.5;
}

.error-state__action {
  max-width: 100%;
}
</style>
