import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/store/modules/auth";

const WORK_ORDER_CHANGED_EVENT = "WorkOrderChanged";

export const WORK_ORDER_REALTIME_STATUSES = {
  connecting: "connecting",
  connected: "connected",
  reconnecting: "reconnecting",
  manual: "manual",
};

export function isWorkOrderRealtimeConfigured() {
  return Boolean(import.meta.env.VITE_SIGNALR_WORKORDERS_HUB);
}

export function getWorkOrderRealtimeStatusLabel(status) {
  if (status === WORK_ORDER_REALTIME_STATUSES.connected) return "Live";
  if (status === WORK_ORDER_REALTIME_STATUSES.connecting) return "Connecting";
  if (status === WORK_ORDER_REALTIME_STATUSES.reconnecting) return "Reconnecting";

  return "Manual refresh";
}

export function getWorkOrderRealtimeStatusTone(status) {
  if (status === WORK_ORDER_REALTIME_STATUSES.connected) return "success";
  if (status === WORK_ORDER_REALTIME_STATUSES.connecting) return "info";
  if (status === WORK_ORDER_REALTIME_STATUSES.reconnecting) return "warning";

  return "neutral";
}

export function createWorkOrderRealtimeConnection({ onChanged, onStatusChange } = {}) {
  let connection = null;
  let isStopping = false;

  function setStatus(status) {
    onStatusChange?.(status);
  }

  async function start() {
    const hubUrl = import.meta.env.VITE_SIGNALR_WORKORDERS_HUB;

    if (!hubUrl) {
      setStatus(WORK_ORDER_REALTIME_STATUSES.manual);
      return false;
    }

    const auth = useAuthStore();

    isStopping = false;
    setStatus(WORK_ORDER_REALTIME_STATUSES.connecting);

    connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => auth.accessToken ?? "",
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on(WORK_ORDER_CHANGED_EVENT, () => {
      onChanged?.();
    });

    connection.onreconnecting(() => {
      setStatus(WORK_ORDER_REALTIME_STATUSES.reconnecting);
    });

    connection.onreconnected(() => {
      setStatus(WORK_ORDER_REALTIME_STATUSES.connected);
      onChanged?.();
    });

    connection.onclose(() => {
      if (!isStopping) {
        setStatus(WORK_ORDER_REALTIME_STATUSES.manual);
      }
    });

    try {
      await connection.start();
      setStatus(WORK_ORDER_REALTIME_STATUSES.connected);
      return true;
    } catch {
      setStatus(WORK_ORDER_REALTIME_STATUSES.manual);
      return false;
    }
  }

  async function stop() {
    isStopping = true;

    if (!connection) {
      setStatus(WORK_ORDER_REALTIME_STATUSES.manual);
      return;
    }

    try {
      await connection.stop();
    } catch {
      // A failed shutdown should not block route changes.
    } finally {
      connection = null;
      setStatus(WORK_ORDER_REALTIME_STATUSES.manual);
    }
  }

  return {
    start,
    stop,
  };
}
