<script setup>
import { ChevronLeft, ChevronRight } from "@lucide/vue";
import ActionButton from "@/components/Shared/ActionButton.vue";

defineProps({
  firstItem: {
    type: Number,
    required: true,
  },
  lastItem: {
    type: Number,
    required: true,
  },
  totalItems: {
    type: Number,
    required: true,
  },
  page: {
    type: Number,
    required: true,
  },
  totalPages: {
    type: Number,
    required: true,
  },
  disabled: {
    type: Boolean,
    default: false,
  },
});

defineEmits(["previous", "next"]);
</script>

<template>
  <footer class="pagination-bar">
    <p>Showing {{ firstItem }}-{{ lastItem }} of {{ totalItems }}</p>

    <div class="pagination-bar__actions">
      <ActionButton
        variant="pager"
        size="sm"
        :disabled="page <= 1 || disabled"
        @click="$emit('previous')"
      >
        <ChevronLeft :size="17" />
        <span>Previous</span>
      </ActionButton>

      <ActionButton
        variant="pager"
        size="sm"
        :disabled="page >= totalPages || disabled"
        @click="$emit('next')"
      >
        <span>Next</span>
        <ChevronRight :size="17" />
      </ActionButton>
    </div>
  </footer>
</template>

<style scoped>
.pagination-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  padding: 16px 18px;
  color: #6b7280;
  background: #fff;
  font-size: 13px;
  font-weight: 700;
}

.pagination-bar p {
  margin: 0;
}

.pagination-bar__actions {
  display: flex;
  gap: 8px;
}

@media (max-width: 720px) {
  .pagination-bar {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
