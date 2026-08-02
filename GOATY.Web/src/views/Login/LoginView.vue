<script setup>
import { Eye, EyeOff, LoaderCircle, LogIn } from "@lucide/vue";
import { computed, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "@/store/modules/auth";
import { getBackendErrorMessage } from "@/utils/api";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const email = ref("");
const password = ref("");
const showPassword = ref(false);
const isLoading = ref(false);
const errorMessage = ref("");

const canSubmit = computed(() => {
  return email.value.trim() && password.value.trim() && !isLoading.value;
});

async function handleSubmit() {
  if (!canSubmit.value) return;

  errorMessage.value = "";
  isLoading.value = true;
  await new Promise((resolve) => setTimeout(resolve, 2000));
  try {
    await auth.login(email.value.trim(), password.value);

    const redirectPath = route.query.redirect?.toString() || "/";
    await router.push(redirectPath);
  } catch (error) {
    errorMessage.value =
      error.response?.status === 401
        ? "Invalid email or password."
        : getBackendErrorMessage(error, "Unable to sign in. Please try again.");
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <main class="login-page">
    <section class="login-shell" aria-label="GOATY sign in">
      <aside class="login-hero" aria-label="Workshop overview">
        <div class="brand-lockup">
          <div class="brand-mark" aria-hidden="true">
            <span class="brand-mark__bolt"></span>
          </div>

          <div>
            <p class="brand-name">GOATY</p>
            <p class="brand-caption">Workshop Command</p>
          </div>
        </div>

        <div class="hero-copy">
          <p class="hero-kicker">Mechanic shop operations</p>
          <h1>Repair flow, schedules, parts, and invoices in one cockpit.</h1>
          <p>
            Keep the floor moving with live work orders, technician assignments,
            bay scheduling, and billing visibility.
          </p>
        </div>

        <div class="hero-metrics" aria-label="Operational highlights">
          <div>
            <span>Live</span>
            <strong>Bay Board</strong>
          </div>
          <div>
            <span>JWT</span>
            <strong>Secured API</strong>
          </div>
          <div>
            <span>SignalR</span>
            <strong>Updates</strong>
          </div>
        </div>
      </aside>

      <section class="login-card">
        <div class="mobile-brand">
          <div class="brand-mark" aria-hidden="true">
            <span class="brand-mark__bolt"></span>
          </div>
          <span>GOATY</span>
        </div>

        <div class="login-header">
          <p class="login-eyebrow">Secure manager access</p>
          <h2>Sign in to your dashboard</h2>
          <p>Use your workshop credentials to continue.</p>
        </div>

        <form class="login-form" @submit.prevent="handleSubmit">
          <div class="field">
            <label for="login-email">Email address</label>
            <input
              id="login-email"
              v-model="email"
              type="email"
              autocomplete="email"
              placeholder="a@gmail.com"
              required
            />
          </div>

          <div class="field">
            <label for="login-password">Password</label>
            <div class="password-control">
              <input
                id="login-password"
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                autocomplete="current-password"
                placeholder="Enter your password"
                required
              />
              <button
                type="button"
                class="password-toggle"
                :aria-label="showPassword ? 'Hide password' : 'Show password'"
                @click="showPassword = !showPassword"
              >
                <EyeOff v-if="showPassword" :size="18" :stroke-width="2.25" />
                <Eye v-else :size="18" :stroke-width="2.25" />
              </button>
            </div>
          </div>

          <div class="form-row">
            <label class="remember-option">
              <input type="checkbox" checked />
              <span>Keep this session ready</span>
            </label>
            <span class="session-note">Dev build</span>
          </div>

          <p v-if="errorMessage" class="error-message">
            {{ errorMessage }}
          </p>

          <button class="login-button" type="submit" :disabled="!canSubmit">
            <span>{{ isLoading ? "Signing in" : "Sign in" }}</span>
            <span class="button-icon" aria-hidden="true">
              <LoaderCircle
                v-if="isLoading"
                class="button-icon__svg button-icon__svg--spin"
                :size="18"
                :stroke-width="2.5"
              />
              <LogIn
                v-else
                class="button-icon__svg"
                :size="18"
                :stroke-width="2.5"
              />
            </span>
          </button>
        </form>
      </section>
    </section>
  </main>
</template>

<style scoped>
.login-page {
  --ink: #10131a;
  --muted: #6b7280;
  --panel: rgba(255, 255, 255, 0.92);
  --line: rgba(17, 24, 39, 0.1);
  --gold: #f59e0b;
  --gold-deep: #b45309;
  --steel: #18202d;
  --steel-soft: #263142;

  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 28px;
  color: var(--ink);
  background:
    radial-gradient(circle at 10% 15%, rgba(245, 158, 11, 0.3), transparent 26%),
    radial-gradient(circle at 88% 5%, rgba(59, 130, 246, 0.16), transparent 22%),
    linear-gradient(135deg, #f8fafc 0%, #e7edf4 45%, #d8e0ea 100%);
}

.login-shell {
  width: min(1180px, 100%);
  min-height: min(760px, calc(100vh - 56px));
  display: grid;
  grid-template-columns: minmax(0, 1.14fr) minmax(390px, 0.86fr);
  overflow: hidden;
  background: rgba(255, 255, 255, 0.68);
  border: 1px solid rgba(255, 255, 255, 0.76);
  border-radius: 8px;
  box-shadow:
    0 28px 90px rgba(15, 23, 42, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.85);
}

.login-hero {
  position: relative;
  isolation: isolate;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-height: 100%;
  padding: 42px;
  color: #f8fafc;
  background:
    linear-gradient(90deg, rgba(9, 12, 18, 0.9), rgba(9, 12, 18, 0.46)),
    linear-gradient(180deg, rgba(9, 12, 18, 0.08), rgba(9, 12, 18, 0.76)),
    url("https://images.unsplash.com/photo-1487754180451-c456f719a1fc?auto=format&fit=crop&w=1800&q=82")
      center / cover;
}

.login-hero::before {
  position: absolute;
  inset: 0;
  z-index: -1;
  content: "";
  background:
    linear-gradient(rgba(255, 255, 255, 0.045) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.045) 1px, transparent 1px);
  background-size: 42px 42px;
  mask-image: linear-gradient(90deg, #000, transparent 84%);
}

.login-hero::after {
  position: absolute;
  right: 36px;
  bottom: 34px;
  width: 190px;
  height: 190px;
  content: "";
  border: 1px solid rgba(255, 255, 255, 0.16);
  border-radius: 999px;
  background:
    radial-gradient(circle, transparent 45%, rgba(255, 255, 255, 0.12) 46%, transparent 47%),
    conic-gradient(from 20deg, rgba(245, 158, 11, 0.45), transparent 18%, rgba(255, 255, 255, 0.22), transparent 42%);
  opacity: 0.9;
}

.brand-lockup,
.mobile-brand {
  display: inline-flex;
  align-items: center;
  gap: 12px;
}

.brand-mark {
  position: relative;
  width: 46px;
  height: 46px;
  flex: 0 0 46px;
  border-radius: 8px;
  background:
    linear-gradient(135deg, #fbbf24, #f97316 58%, #991b1b);
  box-shadow:
    0 14px 26px rgba(245, 158, 11, 0.3),
    inset 0 1px 0 rgba(255, 255, 255, 0.45);
}

.brand-mark::before,
.brand-mark::after {
  position: absolute;
  content: "";
  background: rgba(17, 24, 39, 0.92);
}

.brand-mark::before {
  width: 25px;
  height: 7px;
  left: 10px;
  top: 18px;
  transform: rotate(-34deg);
  border-radius: 999px;
}

.brand-mark::after {
  width: 7px;
  height: 21px;
  right: 11px;
  top: 16px;
  transform: rotate(36deg);
  border-radius: 999px;
}

.brand-mark__bolt {
  position: absolute;
  left: 14px;
  bottom: 10px;
  width: 16px;
  height: 16px;
  border-radius: 999px;
  border: 4px solid rgba(17, 24, 39, 0.92);
}

.brand-name {
  margin: 0;
  font-size: 22px;
  font-weight: 900;
  letter-spacing: 0;
}

.brand-caption {
  margin: 2px 0 0;
  color: rgba(248, 250, 252, 0.68);
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
}

.hero-copy {
  max-width: 650px;
  margin: auto 0;
}

.hero-kicker,
.login-eyebrow {
  margin: 0 0 13px;
  color: var(--gold);
  font-size: 12px;
  font-weight: 900;
  letter-spacing: 0.1em;
  text-transform: uppercase;
}

.hero-copy h1 {
  max-width: 710px;
  margin: 0;
  color: #fff;
  font-size: clamp(42px, 5vw, 74px);
  line-height: 0.94;
  letter-spacing: 0;
}

.hero-copy p:not(.hero-kicker) {
  max-width: 560px;
  margin: 22px 0 0;
  color: rgba(248, 250, 252, 0.76);
  font-size: 17px;
  line-height: 1.7;
}

.hero-metrics {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 12px;
  max-width: 610px;
}

.hero-metrics div {
  min-height: 86px;
  padding: 18px;
  border: 1px solid rgba(255, 255, 255, 0.16);
  border-radius: 8px;
  background: rgba(12, 16, 24, 0.46);
  backdrop-filter: blur(14px);
}

.hero-metrics span {
  display: block;
  color: var(--gold);
  font-size: 12px;
  font-weight: 900;
  text-transform: uppercase;
}

.hero-metrics strong {
  display: block;
  margin-top: 8px;
  color: #fff;
  font-size: 16px;
}

.login-card {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: clamp(30px, 4vw, 54px);
  background:
    radial-gradient(circle at top right, rgba(245, 158, 11, 0.12), transparent 30%),
    var(--panel);
  backdrop-filter: blur(22px);
}

.mobile-brand {
  display: none;
  margin-bottom: 30px;
  color: var(--ink);
  font-size: 19px;
  font-weight: 900;
}

.login-header {
  margin-bottom: 28px;
}

.login-header h2 {
  margin: 0;
  color: var(--ink);
  font-size: clamp(30px, 3vw, 42px);
  line-height: 1.04;
  letter-spacing: 0;
}

.login-header p:last-child {
  margin: 12px 0 0;
  color: var(--muted);
  font-size: 15px;
  line-height: 1.6;
}

.login-form {
  display: grid;
  gap: 18px;
}

.field {
  display: grid;
  gap: 9px;
  text-align: left;
}

.field label {
  color: #374151;
  font-size: 13px;
  font-weight: 800;
}

.field input {
  width: 100%;
  min-height: 52px;
  box-sizing: border-box;
  padding: 0 15px;
  color: var(--ink);
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  outline: none;
  transition:
    border-color 160ms ease,
    box-shadow 160ms ease,
    background 160ms ease;
}

.field input::placeholder {
  color: #9ca3af;
}

.field input:focus {
  background: #fff;
  border-color: rgba(245, 158, 11, 0.78);
  box-shadow:
    0 0 0 4px rgba(245, 158, 11, 0.16),
    0 12px 26px rgba(15, 23, 42, 0.08);
}

.password-control {
  position: relative;
}

.password-control input {
  padding-right: 62px;
}

.password-toggle {
  position: absolute;
  display: inline-grid;
  place-items: center;
  top: 8px;
  right: 8px;
  width: 42px;
  height: 36px;
  border: 0;
  border-radius: 6px;
  color: #4b5563;
  background: #f3f4f6;
  cursor: pointer;
  transition:
    color 160ms ease,
    background 160ms ease,
    box-shadow 160ms ease;
}

.password-toggle:hover {
  color: var(--ink);
  background: #e5e7eb;
}

.password-toggle:focus-visible {
  outline: none;
  box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.22);
}

.form-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  color: var(--muted);
  font-size: 13px;
}

.remember-option {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.remember-option input {
  width: 16px;
  height: 16px;
  accent-color: var(--gold);
}

.session-note {
  padding: 5px 9px;
  color: var(--gold-deep);
  background: rgba(245, 158, 11, 0.12);
  border-radius: 999px;
  font-size: 11px;
  font-weight: 900;
  text-transform: uppercase;
}

.error-message {
  margin: 0;
  padding: 12px 14px;
  color: #991b1b;
  background: #fff1f2;
  border: 1px solid #fecdd3;
  border-radius: 8px;
  font-size: 14px;
  line-height: 1.45;
  text-align: left;
}

.login-button {
  display: inline-flex;
  align-items: center;
  justify-content: space-between;
  min-height: 54px;
  margin-top: 4px;
  padding: 0 9px 0 19px;
  border: 0;
  border-radius: 8px;
  color: #fff;
  background:
    linear-gradient(135deg, rgba(255, 255, 255, 0.16), transparent),
    linear-gradient(135deg, #111827, #263142);
  font: inherit;
  font-weight: 900;
  cursor: pointer;
  box-shadow: 0 16px 30px rgba(15, 23, 42, 0.18);
  transition:
    transform 160ms ease,
    box-shadow 160ms ease,
    opacity 160ms ease;
}

.login-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 22px 34px rgba(15, 23, 42, 0.23);
}

.login-button:disabled {
  cursor: not-allowed;
  opacity: 0.62;
}

.button-icon {
  display: inline-grid;
  place-items: center;
  width: 38px;
  height: 38px;
  margin-left: 12px;
  color: var(--ink);
  background: var(--gold);
  border-radius: 6px;
  font-weight: 900;
}

.button-icon__svg {
  display: block;
}

.button-icon__svg--spin {
  animation: spin 850ms linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.login-footer {
  display: flex;
  align-items: center;
  gap: 9px;
  margin-top: 30px;
  padding-top: 18px;
  color: #6b7280;
  border-top: 1px solid var(--line);
  font-size: 12px;
}

.status-dot {
  width: 9px;
  height: 9px;
  flex: 0 0 9px;
  border-radius: 999px;
  background: #22c55e;
  box-shadow: 0 0 0 5px rgba(34, 197, 94, 0.14);
}

@media (max-width: 900px) {
  .login-page {
    padding: 0;
  }

  .login-shell {
    min-height: 100vh;
    grid-template-columns: 1fr;
    border: 0;
    border-radius: 0;
  }

  .login-hero {
    display: none;
  }

  .login-card {
    padding: 28px;
  }

  .mobile-brand {
    display: inline-flex;
  }
}

@media (max-width: 460px) {
  .login-card {
    padding: 22px;
  }

  .form-row {
    align-items: flex-start;
    flex-direction: column;
  }

  .login-footer {
    align-items: flex-start;
    line-height: 1.45;
  }
}
</style>
