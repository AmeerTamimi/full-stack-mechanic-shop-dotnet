<script setup>
defineProps({
  label: {
    type: String,
    required: true,
  },
  tone: {
    type: String,
    default: "neutral",
  },
  icon: {
    type: [Object, Function],
    default: null,
  },
  size: {
    type: String,
    default: "md",
  },
});
</script>

<template>
  <span class="status-chip" :class="[`status-chip--${tone}`, `status-chip--${size}`]">
    <component v-if="icon" :is="icon" :size="size === 'sm' ? 14 : 16" />
    <span>{{ label }}</span>
  </span>
</template>

<style scoped>
.status-chip {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  width: max-content;
  max-width: 100%;
  border-radius: 999px;
  font-weight: 900;
  white-space: nowrap;
}

.status-chip--md {
  min-height: 30px;
  padding: 0 10px;
  font-size: 12px;
}

.status-chip--sm {
  min-height: 26px;
  padding: 0 8px;
  font-size: 11px;
}

.status-chip--neutral {
  color: #475569;
  background: #f1f5f9;
}

.status-chip--success {
  color: #166534;
  background: #dcfce7;
}

.status-chip--warning {
  color: #92400e;
  background: #fef3c7;
}

.status-chip--danger {
  color: #991b1b;
  background: #fee2e2;
}

.status-chip--info {
  color: #075985;
  background: #e0f2fe;
}

.status-chip--service {
  color: #b91c1c;
  background: #fee2e2;
}

.status-chip--inventory {
  color: #166534;
  background: #dcfce7;
}

.status-chip--people {
  color: #075985;
  background: #e0f2fe;
}

.status-chip--manager {
  color: #92400e;
  background: #fef3c7;
}

.status-chip--customers {
  color: #0f766e;
  background: #ccfbf1;
}
</style>
