<script setup>
import { LoaderCircle } from "@lucide/vue";

defineProps({
  message: {
    type: String,
    default: "Loading...",
  },
});
</script>

<template>
  <div class="loading-state">
    <LoaderCircle class="spinning" :size="22" />
    <span>{{ message }}</span>
  </div>
</template>

<style scoped>
.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  min-height: 210px;
  margin: 18px;
  color: #475569;
  background: #f8fafc;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-weight: 800;
}
</style>
