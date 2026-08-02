<script setup>
import { computed } from "vue";
import { RouterLink } from "vue-router";

defineOptions({
  inheritAttrs: false,
});

const props = defineProps({
  to: {
    type: [String, Object],
    default: null,
  },
  type: {
    type: String,
    default: "button",
  },
  variant: {
    type: String,
    default: "primary",
  },
  size: {
    type: String,
    default: "md",
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  iconOnly: {
    type: Boolean,
    default: false,
  },
  block: {
    type: Boolean,
    default: false,
  },
  ariaLabel: {
    type: String,
    default: "",
  },
});

const buttonClasses = computed(() => [
  "action-button",
  `action-button--${props.variant}`,
  `action-button--${props.size}`,
  {
    "action-button--icon-only": props.iconOnly,
    "action-button--block": props.block,
  },
]);
</script>

<template>
  <RouterLink
    v-if="to && !disabled"
    v-bind="$attrs"
    :to="to"
    :class="buttonClasses"
    :aria-label="ariaLabel || undefined"
  >
    <slot />
  </RouterLink>

  <span
    v-else-if="to && disabled"
    v-bind="$attrs"
    :class="[buttonClasses, 'action-button--disabled']"
    :aria-label="ariaLabel || undefined"
  >
    <slot />
  </span>

  <button
    v-else
    v-bind="$attrs"
    :type="type"
    :disabled="disabled"
    :class="buttonClasses"
    :aria-label="ariaLabel || undefined"
  >
    <slot />
  </button>
</template>

<style scoped>
.action-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-width: 0;
  border: 0;
  border-radius: 8px;
  font: inherit;
  font-weight: 850;
  letter-spacing: 0;
  text-decoration: none;
  cursor: pointer;
  transition:
    transform 160ms ease,
    box-shadow 160ms ease,
    background 160ms ease,
    border-color 160ms ease,
    color 160ms ease,
    opacity 160ms ease;
}

.action-button span {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
}

.action-button--md {
  min-height: 44px;
  padding: 0 15px;
}

.action-button--sm {
  min-height: 38px;
  padding: 0 12px;
}

.action-button--lg {
  min-height: 52px;
  padding: 0 18px;
}

.action-button--icon-only {
  width: 44px;
  padding: 0;
}

.action-button--icon-only.action-button--sm {
  width: 36px;
}

.action-button--block {
  width: 100%;
}

.action-button--primary {
  color: #fff;
  background: linear-gradient(135deg, #111827, #263142);
  box-shadow: 0 14px 28px rgba(15, 23, 42, 0.16);
}

.action-button--secondary {
  color: #374151;
  background: #fff;
  border: 1px solid #e5e7eb;
}

.action-button--ghost {
  color: #374151;
  background: #f3f4f6;
}

.action-button--danger {
  color: #b91c1c;
  background: #f3f4f6;
}

.action-button--pager {
  color: #374151;
  background: #f3f4f6;
}

.action-button:hover:not(:disabled, .action-button--disabled) {
  transform: translateY(-1px);
}

.action-button--primary:hover:not(:disabled, .action-button--disabled) {
  box-shadow: 0 20px 34px rgba(15, 23, 42, 0.21);
}

.action-button--secondary:hover:not(:disabled, .action-button--disabled) {
  color: #111827;
  box-shadow: 0 14px 26px rgba(15, 23, 42, 0.08);
}

.action-button--ghost:hover:not(:disabled, .action-button--disabled),
.action-button--pager:hover:not(:disabled, .action-button--disabled) {
  color: #111827;
  background: #e5e7eb;
}

.action-button--danger:hover:not(:disabled, .action-button--disabled) {
  color: #991b1b;
  background: #fee2e2;
}

.action-button:disabled,
.action-button--disabled {
  cursor: not-allowed;
  opacity: 0.62;
}
</style>
