<script setup>
defineProps({
  id: {
    type: String,
    required: true,
  },
  label: {
    type: String,
    required: true,
  },
  error: {
    type: String,
    default: "",
  },
});
</script>

<template>
  <div class="form-field">
    <label :for="id">{{ label }}</label>
    <slot />
    <p v-if="error" :id="`${id}-error`" class="form-field__error">
      {{ error }}
    </p>
  </div>
</template>

<style scoped>
.form-field {
  display: grid;
  gap: 8px;
}

.form-field label {
  color: #374151;
  font-size: 13px;
  font-weight: 900;
}

.form-field :deep(input),
.form-field :deep(select),
.form-field :deep(textarea) {
  width: 100%;
  min-height: 50px;
  padding: 0 14px;
  color: #111827;
  background: #fff;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  outline: none;
  transition:
    border-color 160ms ease,
    box-shadow 160ms ease;
}

.form-field :deep(select) {
  cursor: pointer;
}

.form-field :deep(textarea) {
  min-height: 116px;
  padding-top: 13px;
  resize: vertical;
}

.form-field :deep(input:focus),
.form-field :deep(select:focus),
.form-field :deep(textarea:focus) {
  border-color: rgba(245, 158, 11, 0.82);
  box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.16);
}

.form-field :deep(input[aria-invalid="true"]),
.form-field :deep(select[aria-invalid="true"]),
.form-field :deep(textarea[aria-invalid="true"]) {
  border-color: #f43f5e;
}

.form-field :deep(input[aria-invalid="true"]:focus),
.form-field :deep(select[aria-invalid="true"]:focus),
.form-field :deep(textarea[aria-invalid="true"]:focus) {
  box-shadow: 0 0 0 4px rgba(244, 63, 94, 0.14);
}

.form-field__error {
  margin: 0;
  color: #be123c;
  font-size: 13px;
  font-weight: 700;
}
</style>
