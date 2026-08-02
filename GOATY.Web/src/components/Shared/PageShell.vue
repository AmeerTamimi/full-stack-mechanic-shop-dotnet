<script setup>
defineProps({
  size: {
    type: String,
    default: "wide",
  },
});
</script>

<template>
  <main class="page-shell" :class="`page-shell--${size}`">
    <slot />
  </main>
</template>

<style scoped>
.page-shell {
  width: min(1180px, 100%);
  margin: 0 auto;
}

.page-shell--form {
  width: min(860px, 100%);
}

.page-shell--dashboard {
  width: min(1180px, 100%);
}
</style>
