const defaultDelayMs = import.meta.env.DEV ? 1000 : 0;
const configuredDelayMs = Number(
  import.meta.env.VITE_OPERATION_DELAY_MS ?? defaultDelayMs
);

export const operationDelayMs =
  Number.isFinite(configuredDelayMs) && configuredDelayMs > 0 ? configuredDelayMs : 0;

export function delay(ms = operationDelayMs) {
  if (!ms) {
    return Promise.resolve();
  }

  return new Promise((resolve) => {
    window.setTimeout(resolve, ms);
  });
}

export async function waitForMinimumDuration(startedAt, minimumMs = operationDelayMs) {
  if (!minimumMs) {
    return;
  }

  const elapsedMs = Date.now() - startedAt;
  const remainingMs = Math.max(0, minimumMs - elapsedMs);

  await delay(remainingMs);
}
