<script setup>
import { Cog, Wrench } from "@lucide/vue";
import { ref, watch } from "vue";
import { useUiStore } from "@/store/modules/ui";

const ui = useUiStore();
const shouldRender = ref(false);
let showTimer = null;

watch(
  () => ui.isGlobalLoaderVisible,
  (isVisible) => {
    window.clearTimeout(showTimer);

    if (isVisible) {
      showTimer = window.setTimeout(() => {
        shouldRender.value = true;
      }, 140);
      return;
    }

    shouldRender.value = false;
  },
  { immediate: true }
);
</script>

<template>
  <Transition name="loader-fade">
    <div v-if="shouldRender" class="global-loader" role="status" aria-live="polite">
      <div class="loader-card">
        <div class="loader-engine" aria-hidden="true">
          <span class="loader-ring"></span>
          <Cog class="loader-gear loader-gear--large" :size="74" :stroke-width="1.9" />
          <Cog class="loader-gear loader-gear--small" :size="38" :stroke-width="2.1" />
          <Wrench class="loader-wrench" :size="42" :stroke-width="2.4" />
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.global-loader {
  position: fixed;
  inset: 0;
  z-index: 9999;
  display: grid;
  place-items: center;
  padding: 24px;
  background:
    radial-gradient(circle at 50% 42%, rgba(245, 158, 11, 0.16), transparent 26%),
    rgba(10, 13, 20, 0.72);
  backdrop-filter: blur(8px);
}

.loader-card {
  position: relative;
  width: 220px;
  height: 220px;
  display: grid;
  place-items: center;
  overflow: hidden;
  padding: 0;
  color: #f8fafc;
  background:
    linear-gradient(135deg, rgba(255, 255, 255, 0.12), transparent),
    linear-gradient(145deg, #121823, #1f2937);
  border: 1px solid rgba(255, 255, 255, 0.14);
  border-radius: 999px;
  box-shadow: 0 28px 80px rgba(0, 0, 0, 0.42);
  text-align: center;
}

.loader-card::before {
  position: absolute;
  inset: 0;
  content: "";
  background:
    linear-gradient(rgba(255, 255, 255, 0.045) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.045) 1px, transparent 1px);
  background-size: 30px 30px;
  mask-image: linear-gradient(#000, transparent 80%);
}

.loader-engine {
  position: relative;
  z-index: 1;
}

.loader-engine {
  width: 142px;
  height: 142px;
}

.loader-ring {
  position: absolute;
  inset: 8px;
  border: 1px solid rgba(245, 158, 11, 0.26);
  border-top-color: #f59e0b;
  border-radius: 999px;
  animation: rotate 1.4s linear infinite;
}

.loader-gear {
  position: absolute;
  color: #f59e0b;
  filter: drop-shadow(0 10px 20px rgba(245, 158, 11, 0.24));
}

.loader-gear--large {
  left: 50%;
  top: 50%;
  animation: rotate-large-gear 1.8s linear infinite;
}

.loader-gear--small {
  right: 18px;
  bottom: 25px;
  color: #fbbf24;
  animation: rotate-reverse 1.25s linear infinite;
}

.loader-wrench {
  position: absolute;
  left: 50%;
  top: 50%;
  color: #f8fafc;
  transform-origin: 50% 50%;
  animation: wrench-pulse 1.25s ease-in-out infinite;
}

.loader-fade-enter-active,
.loader-fade-leave-active {
  transition: opacity 180ms ease;
}

.loader-fade-enter-from,
.loader-fade-leave-to {
  opacity: 0;
}

@keyframes rotate {
  to {
    transform: rotate(360deg);
  }
}

@keyframes rotate-large-gear {
  from {
    transform: translate(-57%, -56%) rotate(0deg);
  }

  to {
    transform: translate(-57%, -56%) rotate(360deg);
  }
}

@keyframes rotate-reverse {
  to {
    transform: rotate(-360deg);
  }
}

@keyframes wrench-pulse {
  0%,
  100% {
    transform: translate(-50%, -50%) rotate(-10deg) scale(1);
  }

  50% {
    transform: translate(-50%, -50%) rotate(8deg) scale(1.05);
  }
}

</style>
