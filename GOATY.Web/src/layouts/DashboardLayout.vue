<script setup>
import { computed } from "vue";
import { RouterLink, RouterView, useRoute, useRouter } from "vue-router";
import { Gauge, LogOut, Package, UserRound, Users, Wrench } from "@lucide/vue";
import { useAuthStore } from "@/store/modules/auth";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const navItems = [
  {
    label: "Dashboard",
    routeName: "home",
    icon: Gauge,
  },
  {
    label: "Parts",
    routeName: "parts",
    icon: Package,
  },
  {
    label: "Repair tasks",
    routeName: "repair-tasks",
    icon: Wrench,
  },
  {
    label: "Employees",
    routeName: "employees",
    icon: Users,
  },
  {
    label: "Customers",
    routeName: "customers",
    icon: UserRound,
  },
];

const currentSection = computed(() => {
  return route.meta.title ?? navItems.find((item) => item.routeName === route.name)?.label ?? "GOATY";
});

function handleLogout() {
  auth.logout();
  router.push({ name: "login" });
}
</script>

<template>
  <div class="dashboard-layout">
    <header class="app-header">
      <div class="header-shell">
        <RouterLink class="brand" :to="{ name: 'home' }">
          <span class="brand-mark">
            <Wrench :size="22" />
          </span>
          <span class="brand-copy">
            <strong>GOATY</strong>
            <small>Service workbench</small>
          </span>
        </RouterLink>

        <nav class="nav-rail" aria-label="Main navigation">
          <RouterLink
            v-for="item in navItems"
            :key="item.routeName"
            class="nav-link"
            :to="{ name: item.routeName }"
          >
            <component :is="item.icon" :size="18" />
            <span>{{ item.label }}</span>
          </RouterLink>
        </nav>

        <div class="header-tools">
          <div class="section-chip" aria-label="Current section">
            <span class="section-chip__dot"></span>
            <span>{{ currentSection }}</span>
          </div>

          <button class="logout-button" type="button" @click="handleLogout">
            <LogOut :size="18" />
            <span>Logout</span>
          </button>
        </div>
      </div>
    </header>

    <main class="layout-main">
      <div class="layout-content">
        <RouterView />
      </div>
    </main>
  </div>
</template>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
  color: #111827;
  background:
    linear-gradient(135deg, rgba(245, 158, 11, 0.1), transparent 32%),
    linear-gradient(180deg, #f8fafc 0%, #edf2f7 100%);
}

.dashboard-layout::before {
  position: fixed;
  inset: 0;
  z-index: 0;
  pointer-events: none;
  content: "";
  background-image:
    linear-gradient(rgba(15, 23, 42, 0.045) 1px, transparent 1px),
    linear-gradient(90deg, rgba(15, 23, 42, 0.045) 1px, transparent 1px);
  background-size: 42px 42px;
  mask-image: linear-gradient(180deg, #000 0%, transparent 68%);
}

.app-header {
  position: sticky;
  top: 0;
  z-index: 20;
  padding: 14px 24px 0;
  background: linear-gradient(180deg, rgba(248, 250, 252, 0.96), rgba(248, 250, 252, 0.78));
  backdrop-filter: blur(18px);
}

.header-shell {
  position: relative;
  display: grid;
  grid-template-columns: minmax(190px, 0.9fr) auto minmax(260px, 0.9fr);
  align-items: center;
  gap: 18px;
  width: min(1220px, 100%);
  min-height: 72px;
  margin: 0 auto;
  padding: 10px;
  background:
    linear-gradient(135deg, rgba(17, 24, 39, 0.98), rgba(31, 41, 55, 0.96)),
    repeating-linear-gradient(135deg, transparent 0 18px, rgba(255, 255, 255, 0.05) 18px 19px);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 14px;
  box-shadow:
    0 24px 52px rgba(15, 23, 42, 0.18),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

.header-shell::after {
  position: absolute;
  right: 18px;
  bottom: -4px;
  left: 18px;
  height: 4px;
  content: "";
  background: repeating-linear-gradient(
    90deg,
    #f59e0b 0 28px,
    #111827 28px 40px,
    #64748b 40px 42px
  );
  border-radius: 999px;
}

.brand {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
  min-height: 52px;
  color: #fff;
  text-decoration: none;
}

.brand-mark {
  position: relative;
  display: grid;
  place-items: center;
  width: 48px;
  height: 48px;
  flex: 0 0 48px;
  color: #111827;
  background: #f59e0b;
  border-radius: 12px;
  box-shadow:
    0 12px 24px rgba(245, 158, 11, 0.25),
    inset 0 -3px 0 rgba(120, 53, 15, 0.22);
}

.brand-mark::before {
  position: absolute;
  inset: 8px;
  content: "";
  border: 1px solid rgba(17, 24, 39, 0.25);
  border-radius: 9px;
}

.brand-copy {
  min-width: 0;
}

.brand strong,
.brand small {
  display: block;
}

.brand strong {
  color: #fff;
  font-size: 20px;
  letter-spacing: 0;
}

.brand small {
  margin-top: 2px;
  overflow: hidden;
  color: #a7f3d0;
  font-size: 12px;
  font-weight: 850;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.nav-rail {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 6px;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 999px;
}

.nav-link,
.logout-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 40px;
  border: 0;
  border-radius: 999px;
  font: inherit;
  font-size: 14px;
  font-weight: 850;
  letter-spacing: 0;
  text-decoration: none;
  cursor: pointer;
  transition:
    transform 160ms ease,
    background 160ms ease,
    color 160ms ease,
    box-shadow 160ms ease;
}

.nav-link {
  padding: 0 14px;
  color: #d1d5db;
}

.nav-link:hover {
  transform: translateY(-1px);
  color: #fff;
  background: rgba(255, 255, 255, 0.1);
}

.nav-link.router-link-active {
  color: #111827;
  background: #f59e0b;
  box-shadow: 0 10px 22px rgba(245, 158, 11, 0.22);
}

.header-tools {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 10px;
  min-width: 0;
}

.section-chip {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  min-height: 40px;
  min-width: 0;
  padding: 0 12px;
  color: #e5e7eb;
  background: rgba(15, 23, 42, 0.62);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 999px;
  font-size: 13px;
  font-weight: 850;
}

.section-chip span:last-child {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.section-chip__dot {
  width: 8px;
  height: 8px;
  flex: 0 0 8px;
  background: #22c55e;
  border-radius: 999px;
  box-shadow: 0 0 0 5px rgba(34, 197, 94, 0.14);
}

.logout-button {
  padding: 0 13px;
  color: #fee2e2;
  background: rgba(239, 68, 68, 0.12);
}

.logout-button:hover {
  transform: translateY(-1px);
  color: #fff;
  background: rgba(239, 68, 68, 0.22);
}

.layout-main {
  position: relative;
  z-index: 1;
  min-width: 0;
}

.layout-content {
  width: min(1260px, 100%);
  margin: 0 auto;
  padding: 34px 32px 42px;
}

@media (max-width: 980px) {
  .header-shell {
    grid-template-columns: 1fr;
  }

  .nav-rail {
    justify-content: flex-start;
    overflow-x: auto;
  }

  .header-tools {
    justify-content: space-between;
  }
}

@media (max-width: 720px) {
  .app-header {
    padding: 10px 12px 0;
  }

  .header-shell {
    border-radius: 12px;
  }

  .brand {
    width: 100%;
  }

  .header-tools {
    align-items: stretch;
    flex-direction: column;
  }

  .section-chip,
  .logout-button {
    width: 100%;
  }

  .layout-content {
    padding: 24px 18px 34px;
  }
}
</style>
