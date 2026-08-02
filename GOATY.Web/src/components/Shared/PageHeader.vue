<script setup>
defineProps({
  eyebrow: {
    type: String,
    default: "",
  },
  title: {
    type: String,
    required: true,
  },
  subtitle: {
    type: String,
    default: "",
  },
  icon: {
    type: [Object, Function],
    default: null,
  },
  tone: {
    type: String,
    default: "default",
  },
});
</script>

<template>
  <header class="page-header" :class="`page-header--${tone}`">
    <div class="page-header__content">
      <div v-if="icon" class="page-header__icon" aria-hidden="true">
        <component :is="icon" :size="27" />
      </div>

      <div class="page-header__copy">
        <p v-if="eyebrow" class="page-header__eyebrow">{{ eyebrow }}</p>
        <h1>{{ title }}</h1>
        <p v-if="subtitle" class="page-header__subtitle">{{ subtitle }}</p>
      </div>
    </div>

    <div v-if="$slots.actions" class="page-header__actions">
      <slot name="actions" />
    </div>
  </header>
</template>

<style scoped>
.page-header {
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  margin-bottom: 22px;
  padding: 20px;
  color: #fff;
  background:
    linear-gradient(135deg, rgba(15, 23, 42, 0.98), rgba(30, 41, 59, 0.96)),
    repeating-linear-gradient(135deg, transparent 0 18px, rgba(255, 255, 255, 0.05) 18px 19px);
  border: 1px solid rgba(255, 255, 255, 0.11);
  border-radius: 8px;
  box-shadow:
    0 22px 50px rgba(15, 23, 42, 0.16),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

.page-header::before {
  position: absolute;
  inset: 0 auto 0 0;
  width: 7px;
  content: "";
  background: var(--page-accent, #f59e0b);
}

.page-header::after {
  position: absolute;
  right: 18px;
  bottom: 12px;
  left: 80px;
  height: 1px;
  content: "";
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.16), transparent);
}

.page-header--dashboard {
  --page-accent: #22c55e;
  --page-accent-soft: rgba(34, 197, 94, 0.16);
}

.page-header--inventory {
  --page-accent: #f59e0b;
  --page-accent-soft: rgba(245, 158, 11, 0.17);
}

.page-header--people {
  --page-accent: #38bdf8;
  --page-accent-soft: rgba(56, 189, 248, 0.16);
}

.page-header--customers {
  --page-accent: #14b8a6;
  --page-accent-soft: rgba(20, 184, 166, 0.16);
}

.page-header--service {
  --page-accent: #ef4444;
  --page-accent-soft: rgba(239, 68, 68, 0.16);
}

.page-header__content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  gap: 16px;
  min-width: 0;
}

.page-header__icon {
  position: relative;
  display: grid;
  place-items: center;
  width: 58px;
  height: 58px;
  flex: 0 0 58px;
  color: var(--page-accent, #f59e0b);
  background: var(--page-accent-soft, rgba(245, 158, 11, 0.17));
  border: 1px solid color-mix(in srgb, var(--page-accent, #f59e0b) 42%, transparent);
  border-radius: 8px;
  box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.05);
}

.page-header__icon::after {
  position: absolute;
  inset: 9px;
  content: "";
  border: 1px solid color-mix(in srgb, var(--page-accent, #f59e0b) 24%, transparent);
  border-radius: 6px;
}

.page-header__copy {
  min-width: 0;
}

.page-header__eyebrow {
  margin: 0 0 8px;
  color: var(--page-accent, #f59e0b);
  font-size: 12px;
  font-weight: 900;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.page-header h1 {
  margin: 0;
  color: #fff;
  font-size: 38px;
  line-height: 1.02;
  letter-spacing: 0;
}

.page-header__subtitle {
  margin: 10px 0 0;
  color: #cbd5e1;
  font-size: 15px;
  line-height: 1.5;
}

.page-header__actions {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
  justify-content: flex-end;
  min-width: 0;
  max-width: 100%;
}

.page-header__actions :deep(.action-button--primary) {
  color: #111827;
  background: var(--page-accent, #f59e0b);
  box-shadow: 0 16px 28px color-mix(in srgb, var(--page-accent, #f59e0b) 18%, transparent);
}

.page-header__actions :deep(.action-button--secondary),
.page-header__actions :deep(.action-button--ghost) {
  color: #f8fafc;
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.14);
}

.page-header__actions :deep(.action-button--secondary:hover),
.page-header__actions :deep(.action-button--ghost:hover) {
  color: #fff;
  background: rgba(255, 255, 255, 0.16);
}

@media (max-width: 720px) {
  .page-header {
    align-items: stretch;
    flex-direction: column;
    padding: 18px;
  }

  .page-header__actions {
    width: 100%;
    justify-content: flex-start;
  }

  .page-header__actions :deep(.action-button),
  .page-header__actions :deep(.status-chip) {
    max-width: 100%;
  }

  .page-header__content {
    align-items: flex-start;
    flex-direction: column;
  }

  .page-header h1 {
    font-size: 30px;
  }
}
</style>
