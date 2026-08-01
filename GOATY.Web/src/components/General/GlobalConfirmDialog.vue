<script setup>
import { AlertTriangle, X } from "@lucide/vue";
import { computed } from "vue";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();

const dialog = computed(() => ui.confirmDialog);
</script>

<template>
  <Transition name="confirm-fade">
    <div
      v-if="dialog.visible"
      class="confirm-backdrop"
      role="presentation"
      @click.self="ui.resolveConfirm(false)"
    >
      <section class="confirm-dialog" role="dialog" aria-modal="true" aria-labelledby="confirm-title">
        <button
          class="confirm-close"
          type="button"
          aria-label="Close confirmation dialog"
          @click="ui.resolveConfirm(false)"
        >
          <X :size="17" />
        </button>

        <div class="confirm-icon" :class="`confirm-icon--${dialog.variant}`" aria-hidden="true">
          <AlertTriangle :size="26" />
        </div>

        <div class="confirm-copy">
          <h2 id="confirm-title">{{ dialog.title }}</h2>
          <p>{{ dialog.message }}</p>
        </div>

        <div class="confirm-actions">
          <button class="confirm-button confirm-button--secondary" type="button" @click="ui.resolveConfirm(false)">
            {{ dialog.cancelText }}
          </button>
          <button
            class="confirm-button confirm-button--primary"
            :class="`confirm-button--${dialog.variant}`"
            type="button"
            @click="ui.resolveConfirm(true)"
          >
            {{ dialog.confirmText }}
          </button>
        </div>
      </section>
    </div>
  </Transition>
</template>

<style scoped>
.confirm-backdrop {
  position: fixed;
  inset: 0;
  z-index: 10001;
  display: grid;
  place-items: center;
  padding: 24px;
  background: rgba(15, 23, 42, 0.58);
  backdrop-filter: blur(6px);
}

.confirm-dialog {
  position: relative;
  width: min(430px, 100%);
  padding: 28px;
  color: #111827;
  background: #fff;
  border: 1px solid rgba(17, 24, 39, 0.1);
  border-radius: 8px;
  box-shadow: 0 28px 80px rgba(15, 23, 42, 0.32);
}

.confirm-close {
  position: absolute;
  top: 14px;
  right: 14px;
  display: inline-grid;
  place-items: center;
  width: 32px;
  height: 32px;
  border: 0;
  border-radius: 6px;
  color: #64748b;
  background: #f8fafc;
  cursor: pointer;
}

.confirm-close:hover {
  color: #111827;
  background: #eef2f7;
}

.confirm-icon {
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  margin-bottom: 18px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.13);
  border-radius: 8px;
}

.confirm-icon--danger {
  color: #b91c1c;
  background: #fee2e2;
}

.confirm-copy h2 {
  margin: 0;
  color: #111827;
  font-size: 24px;
  line-height: 1.15;
  letter-spacing: 0;
}

.confirm-copy p {
  margin: 10px 0 0;
  color: #64748b;
  font-size: 14px;
  line-height: 1.55;
}

.confirm-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 24px;
}

.confirm-button {
  min-height: 42px;
  padding: 0 15px;
  border: 0;
  border-radius: 8px;
  font: inherit;
  font-weight: 850;
  cursor: pointer;
}

.confirm-button--secondary {
  color: #374151;
  background: #f3f4f6;
}

.confirm-button--secondary:hover {
  color: #111827;
  background: #e5e7eb;
}

.confirm-button--primary {
  color: #fff;
  background: #111827;
}

.confirm-button--danger {
  color: #fff;
  background: #dc2626;
}

.confirm-button--danger:hover {
  background: #b91c1c;
}

.confirm-fade-enter-active,
.confirm-fade-leave-active {
  transition: opacity 160ms ease;
}

.confirm-fade-enter-from,
.confirm-fade-leave-to {
  opacity: 0;
}

@media (max-width: 480px) {
  .confirm-dialog {
    padding: 22px;
  }

  .confirm-actions {
    flex-direction: column-reverse;
  }

  .confirm-button {
    width: 100%;
  }
}
</style>
