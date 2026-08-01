<script setup>
import { AlertCircle, CheckCircle2, Info, X } from "@lucide/vue";
import { computed } from "vue";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();

const icons = {
  success: CheckCircle2,
  error: AlertCircle,
  info: Info,
};

function getToastIcon(type) {
  return icons[type] ?? Info;
}

const toasts = computed(() => ui.toasts);
</script>

<template>
  <div class="toast-region" aria-live="polite" aria-label="Notifications">
    <TransitionGroup name="toast" tag="div" class="toast-stack">
      <article
        v-for="toast in toasts"
        :key="toast.id"
        class="toast-card"
        :class="`toast-card--${toast.type}`"
      >
        <component :is="getToastIcon(toast.type)" class="toast-icon" :size="20" />

        <div class="toast-copy">
          <strong>{{ toast.title }}</strong>
          <p v-if="toast.message">{{ toast.message }}</p>
        </div>

        <button
          class="toast-close"
          type="button"
          aria-label="Dismiss notification"
          @click="ui.removeToast(toast.id)"
        >
          <X :size="16" />
        </button>
      </article>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.toast-region {
  position: fixed;
  top: 18px;
  right: 18px;
  z-index: 10000;
  width: min(390px, calc(100vw - 36px));
  pointer-events: none;
}

.toast-stack {
  display: grid;
  gap: 12px;
}

.toast-card {
  display: grid;
  grid-template-columns: auto minmax(0, 1fr) auto;
  gap: 12px;
  align-items: flex-start;
  padding: 14px;
  color: #111827;
  background: rgba(255, 255, 255, 0.96);
  border: 1px solid rgba(17, 24, 39, 0.1);
  border-left: 4px solid #64748b;
  border-radius: 8px;
  box-shadow: 0 18px 42px rgba(15, 23, 42, 0.18);
  pointer-events: auto;
}

.toast-card--success {
  border-left-color: #16a34a;
}

.toast-card--error {
  border-left-color: #dc2626;
}

.toast-card--info {
  border-left-color: #2563eb;
}

.toast-icon {
  margin-top: 2px;
  color: #64748b;
}

.toast-card--success .toast-icon {
  color: #16a34a;
}

.toast-card--error .toast-icon {
  color: #dc2626;
}

.toast-card--info .toast-icon {
  color: #2563eb;
}

.toast-copy strong {
  display: block;
  color: #111827;
  font-size: 14px;
  line-height: 1.3;
}

.toast-copy p {
  margin: 4px 0 0;
  color: #64748b;
  font-size: 13px;
  line-height: 1.45;
}

.toast-close {
  display: inline-grid;
  place-items: center;
  width: 28px;
  height: 28px;
  border: 0;
  border-radius: 6px;
  color: #64748b;
  background: transparent;
  cursor: pointer;
}

.toast-close:hover {
  color: #111827;
  background: #f1f5f9;
}

.toast-enter-active,
.toast-leave-active {
  transition:
    opacity 180ms ease,
    transform 180ms ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
