<script setup>
defineProps({
  title: {
    type: String,
    required: true,
  },
  message: {
    type: String,
    default: "",
  },
});
</script>

<template>
  <div class="empty-state">
    <div v-if="$slots.icon" class="empty-state__icon">
      <slot name="icon" />
    </div>
    <h2>{{ title }}</h2>
    <p v-if="message">{{ message }}</p>
    <div v-if="$slots.action" class="empty-state__action">
      <slot name="action" />
    </div>
  </div>
</template>

<style scoped>
.empty-state {
  display: grid;
  justify-items: center;
  gap: 12px;
  padding: 64px 24px;
  text-align: center;
}

.empty-state__icon {
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.12);
  border-radius: 8px;
}

.empty-state h2 {
  margin: 0;
  max-width: 100%;
  overflow-wrap: anywhere;
  color: #111827;
  font-size: 24px;
}

.empty-state p {
  max-width: 390px;
  margin: 0 0 8px;
  overflow-wrap: anywhere;
  color: #6b7280;
}

.empty-state__action {
  max-width: 100%;
}
</style>
