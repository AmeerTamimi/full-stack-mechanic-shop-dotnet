<script setup>
defineProps({
  title: {
    type: String,
    required: true,
  },
  subtitle: {
    type: String,
    default: "",
  },
});
</script>

<template>
  <section class="form-panel">
    <div class="form-panel__intro">
      <div v-if="$slots.icon" class="form-panel__icon" aria-hidden="true">
        <slot name="icon" />
      </div>
      <div>
        <h2>{{ title }}</h2>
        <p v-if="subtitle">{{ subtitle }}</p>
      </div>
    </div>

    <slot />
  </section>
</template>

<style scoped>
.form-panel {
  padding: 28px;
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 20px 48px rgba(15, 23, 42, 0.1);
}

.form-panel__intro {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 24px;
  padding-bottom: 22px;
  border-bottom: 1px solid #eef2f7;
}

.form-panel__icon {
  display: grid;
  place-items: center;
  width: 54px;
  height: 54px;
  flex: 0 0 54px;
  color: #b45309;
  background: rgba(245, 158, 11, 0.13);
  border-radius: 8px;
}

.form-panel h2 {
  margin: 0;
  color: #111827;
  font-size: 24px;
  letter-spacing: 0;
}

.form-panel p {
  margin: 6px 0 0;
  color: #6b7280;
  font-size: 14px;
}

@media (max-width: 720px) {
  .form-panel__intro {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>
