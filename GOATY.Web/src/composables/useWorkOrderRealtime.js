import { computed, onBeforeUnmount, onMounted, ref, unref, watch } from "vue";
import {
  createWorkOrderRealtimeConnection,
  isWorkOrderRealtimeConfigured,
  WORK_ORDER_REALTIME_STATUSES,
} from "@/services/workOrderRealtime.service";

export function useWorkOrderRealtime({ enabled = true, onChanged, debounceMs = 450 } = {}) {
  const realtimeStatus = ref(WORK_ORDER_REALTIME_STATUSES.manual);
  const lastRealtimeChangeAt = ref("");
  let controls = null;
  let refreshTimer = null;
  let isStarting = false;

  function isEnabled() {
    return Boolean(unref(enabled));
  }

  function queueChange() {
    lastRealtimeChangeAt.value = new Date().toISOString();
    window.clearTimeout(refreshTimer);
    refreshTimer = window.setTimeout(() => {
      onChanged?.();
    }, debounceMs);
  }

  async function start() {
    if (!isEnabled() || controls || isStarting) {
      return;
    }

    isStarting = true;
    controls = createWorkOrderRealtimeConnection({
      onChanged: queueChange,
      onStatusChange: (status) => {
        realtimeStatus.value = status;
      },
    });

    await controls.start();
    isStarting = false;
  }

  async function stop() {
    window.clearTimeout(refreshTimer);
    refreshTimer = null;
    isStarting = false;

    const activeControls = controls;
    controls = null;

    if (activeControls) {
      await activeControls.stop();
    } else {
      realtimeStatus.value = WORK_ORDER_REALTIME_STATUSES.manual;
    }
  }

  onMounted(start);
  onBeforeUnmount(stop);

  watch(
    () => isEnabled(),
    (shouldRun) => {
      if (shouldRun) {
        start();
        return;
      }

      stop();
    }
  );

  return {
    isRealtimeConfigured: computed(() => isWorkOrderRealtimeConfigured()),
    lastRealtimeChangeAt: computed(() => lastRealtimeChangeAt.value),
    realtimeStatus: computed(() => realtimeStatus.value),
    startRealtime: start,
    stopRealtime: stop,
  };
}
