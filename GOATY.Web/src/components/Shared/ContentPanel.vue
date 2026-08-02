<template>
  <section class="content-panel">
    <slot />
  </section>
</template>

<style scoped>
.content-panel {
  overflow: hidden;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 20px 48px rgba(15, 23, 42, 0.1);
}
</style>
