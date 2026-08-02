<script setup>
import { CalendarDays, Gauge, LogOut, ShieldAlert } from "@lucide/vue";
import { computed } from "vue";
import { useRouter } from "vue-router";
import ActionButton from "@/components/Shared/ActionButton.vue";
import { useAuthStore } from "@/store/modules/auth";

const router = useRouter();
const auth = useAuthStore();
const hasDefaultRoute = computed(() => auth.isManager || auth.isTechnician);
const defaultRoute = computed(() => (auth.isManager ? { name: "home" } : { name: "schedule" }));
const defaultRouteLabel = computed(() => (auth.isManager ? "Go to dashboard" : "Go to schedule"));
const DefaultRouteIcon = computed(() => (auth.isManager ? Gauge : CalendarDays));

async function signOut() {
  auth.logout();
  await router.push({ name: "login" });
}
</script>

<template>
  <main class="system-page">
    <section class="system-panel">
      <span class="system-panel__icon system-panel__icon--danger">
        <ShieldAlert :size="34" />
      </span>

      <p class="system-panel__eyebrow">Access blocked</p>
      <h1>You do not have permission to view this page.</h1>
      <p>
        This area is only available to roles with the right workshop access.
      </p>

      <div class="system-panel__actions">
        <ActionButton v-if="auth.isAuthenticated && hasDefaultRoute" :to="defaultRoute">
          <component :is="DefaultRouteIcon" :size="18" />
          <span>{{ defaultRouteLabel }}</span>
        </ActionButton>

        <ActionButton variant="secondary" @click="signOut">
          <LogOut :size="18" />
          <span>Sign out</span>
        </ActionButton>
      </div>
    </section>
  </main>
</template>

<style scoped>
.system-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 28px;
  background:
    linear-gradient(135deg, rgba(239, 68, 68, 0.11), transparent 36%),
    linear-gradient(180deg, #f8fafc, #e5edf6);
}

.system-panel {
  width: min(560px, 100%);
  padding: 34px;
  text-align: center;
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid rgba(17, 24, 39, 0.08);
  border-radius: 8px;
  box-shadow: 0 24px 58px rgba(15, 23, 42, 0.13);
}

.system-panel__icon {
  display: inline-grid;
  place-items: center;
  width: 70px;
  height: 70px;
  margin-bottom: 18px;
  border-radius: 8px;
}

.system-panel__icon--danger {
  color: #b91c1c;
  background: #fee2e2;
}

.system-panel__eyebrow {
  margin: 0 0 10px;
  color: #b91c1c;
  font-size: 12px;
  font-weight: 900;
  letter-spacing: 0.1em;
  text-transform: uppercase;
}

.system-panel h1 {
  margin: 0;
  color: #111827;
  font-size: 34px;
  line-height: 1.08;
}

.system-panel p:last-of-type {
  max-width: 410px;
  margin: 14px auto 0;
  color: #64748b;
  line-height: 1.6;
}

.system-panel__actions {
  display: flex;
  justify-content: center;
  gap: 10px;
  flex-wrap: wrap;
  margin-top: 26px;
}
</style>
