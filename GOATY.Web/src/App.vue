<script setup>
import GlobalConfirmDialog from "@/components/General/GlobalConfirmDialog.vue";
import GlobalLoader from "@/components/General/GlobalLoader.vue";
import GlobalToast from "@/components/General/GlobalToast.vue";
</script>

<template>
  <GlobalLoader />
  <GlobalToast />
  <GlobalConfirmDialog />
  <router-view />
</template>
