<script setup>
import { Search, X } from "@lucide/vue";
import ActionButton from "@/components/Shared/ActionButton.vue";

const props = defineProps({
  search: {
    type: String,
    default: "",
  },
  searchPlaceholder: {
    type: String,
    default: "Search",
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  resultLabel: {
    type: String,
    default: "",
  },
});

const emit = defineEmits(["update:search", "submit", "clear"]);

function updateSearch(event) {
  emit("update:search", event.target.value);
}

function clearSearch() {
  emit("update:search", "");
  emit("clear");
}
</script>

<template>
  <form class="entity-filter-bar" role="search" @submit.prevent="$emit('submit')">
    <label class="entity-filter-bar__search">
      <Search :size="18" />
      <input
        :value="search"
        type="search"
        :placeholder="searchPlaceholder"
        :disabled="disabled"
        @input="updateSearch"
      />
      <button
        v-if="search"
        class="entity-filter-bar__clear"
        type="button"
        :disabled="disabled"
        aria-label="Clear search"
        @click="clearSearch"
      >
        <X :size="16" />
      </button>
    </label>

    <div v-if="$slots.filters" class="entity-filter-bar__filters">
      <slot name="filters" />
    </div>

    <div class="entity-filter-bar__actions">
      <span v-if="resultLabel" class="entity-filter-bar__result">{{ resultLabel }}</span>
      <slot name="actions">
        <ActionButton variant="secondary" type="submit" :disabled="disabled">
          <Search :size="17" />
          <span>Search</span>
        </ActionButton>
      </slot>
    </div>
  </form>
</template>

<style scoped>
.entity-filter-bar {
  display: grid;
  grid-template-columns: minmax(220px, 1fr) auto;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding: 14px;
  background: rgba(255, 255, 255, 0.82);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 10px 26px rgba(15, 23, 42, 0.06);
}

.entity-filter-bar__search {
  position: relative;
  display: flex;
  align-items: center;
  gap: 9px;
  min-width: 0;
  min-height: 44px;
  padding: 0 12px;
  color: #64748b;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 8px;
}

.entity-filter-bar__search input {
  width: 100%;
  min-width: 0;
  color: #111827;
  background: transparent;
  border: 0;
  outline: none;
}

.entity-filter-bar__search:focus-within {
  border-color: rgba(245, 158, 11, 0.82);
  box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.14);
}

.entity-filter-bar__clear {
  display: inline-grid;
  place-items: center;
  width: 28px;
  height: 28px;
  flex: 0 0 28px;
  color: #64748b;
  background: #f1f5f9;
  border: 0;
  border-radius: 6px;
  cursor: pointer;
}

.entity-filter-bar__filters,
.entity-filter-bar__actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.entity-filter-bar__actions {
  justify-content: flex-end;
}

.entity-filter-bar__result {
  color: #64748b;
  font-size: 13px;
  font-weight: 850;
  white-space: nowrap;
}

@media (max-width: 760px) {
  .entity-filter-bar {
    grid-template-columns: 1fr;
  }

  .entity-filter-bar__filters,
  .entity-filter-bar__actions {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
