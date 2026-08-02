<script setup>
import { computed } from "vue";
import ContentPanel from "@/components/Shared/ContentPanel.vue";
import EmptyState from "@/components/Shared/EmptyState.vue";
import ErrorState from "@/components/Shared/ErrorState.vue";
import LoadingState from "@/components/Shared/LoadingState.vue";
import PaginationBar from "@/components/Shared/PaginationBar.vue";

const props = defineProps({
  isLoading: {
    type: Boolean,
    default: false,
  },
  hasItems: {
    type: Boolean,
    default: false,
  },
  loadingMessage: {
    type: String,
    default: "Loading...",
  },
  emptyTitle: {
    type: String,
    default: "Nothing here yet",
  },
  emptyMessage: {
    type: String,
    default: "",
  },
  errorMessage: {
    type: String,
    default: "",
  },
  errorTitle: {
    type: String,
    default: "Unable to load data",
  },
  firstItem: {
    type: Number,
    default: 0,
  },
  lastItem: {
    type: Number,
    default: 0,
  },
  totalItems: {
    type: Number,
    default: 0,
  },
  page: {
    type: Number,
    default: 1,
  },
  totalPages: {
    type: Number,
    default: 1,
  },
  paginationDisabled: {
    type: Boolean,
    default: false,
  },
});

defineEmits(["previous", "next", "retry"]);

const shouldShowPagination = computed(() => {
  return props.hasItems && props.totalItems > 0;
});
</script>

<template>
  <div class="entity-list-shell">
    <slot name="filters" />

    <ContentPanel>
      <ErrorState
        v-if="errorMessage"
        :title="errorTitle"
        :message="errorMessage"
        @retry="$emit('retry')"
      >
        <template v-if="$slots.errorIcon" #icon>
          <slot name="errorIcon" />
        </template>
        <template v-if="$slots.errorAction" #action>
          <slot name="errorAction" />
        </template>
      </ErrorState>

      <LoadingState v-else-if="isLoading && !hasItems" :message="loadingMessage" />

      <EmptyState v-else-if="!hasItems" :title="emptyTitle" :message="emptyMessage">
        <template v-if="$slots.emptyIcon" #icon>
          <slot name="emptyIcon" />
        </template>
        <template v-if="$slots.emptyAction" #action>
          <slot name="emptyAction" />
        </template>
      </EmptyState>

      <template v-else>
        <slot />
      </template>

      <PaginationBar
        v-if="shouldShowPagination"
        :first-item="firstItem"
        :last-item="lastItem"
        :total-items="totalItems"
        :page="page"
        :total-pages="totalPages"
        :disabled="paginationDisabled || isLoading"
        @previous="$emit('previous')"
        @next="$emit('next')"
      />
    </ContentPanel>
  </div>
</template>
