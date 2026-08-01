import { defineStore } from "pinia";

let toastId = 0;
let confirmResolver = null;

export const useUiStore = defineStore("ui", {
  state: () => ({
    showLoaderCount: 0,
    sidebarVisible: false,
    datetimeDialogDisplayed: false,
    notificationBarDismissed: false,
    toasts: [],
    confirmDialog: {
      visible: false,
      title: "",
      message: "",
      confirmText: "Confirm",
      cancelText: "Cancel",
      variant: "danger",
    },
  }),

  getters: {
    isGlobalLoaderVisible: (state) => state.showLoaderCount > 0,
  },

  actions: {
    changeShowLoader(payload) {
      if (payload === true) {
        this.showLoaderCount += 1;
        return;
      }

      this.showLoaderCount = Math.max(0, this.showLoaderCount - 1);
    },

    resetLoader() {
      this.showLoaderCount = 0;
    },

    changeDatetimeDialogDisplayed(payload) {
      this.datetimeDialogDisplayed = payload;
    },

    changeNotificationBarDismissedStatus(payload) {
      this.notificationBarDismissed = payload;
    },

    async withLoader(work) {
      this.changeShowLoader(true);

      try {
        return await work();
      } finally {
        this.changeShowLoader(false);
      }
    },

    addToast({ type = "info", title = "", message = "", duration = 4000 } = {}) {
      const id = ++toastId;

      this.toasts.push({
        id,
        type,
        title,
        message,
      });

      if (duration > 0) {
        window.setTimeout(() => {
          this.removeToast(id);
        }, duration);
      }

      return id;
    },

    removeToast(id) {
      this.toasts = this.toasts.filter((toast) => toast.id !== id);
    },

    showSuccessToast(message, title = "Success") {
      return this.addToast({
        type: "success",
        title,
        message,
      });
    },

    showErrorToast(message, title = "Something went wrong") {
      return this.addToast({
        type: "error",
        title,
        message,
        duration: 6000,
      });
    },

    confirm(options = {}) {
      if (confirmResolver) {
        confirmResolver(false);
      }

      this.confirmDialog = {
        visible: true,
        title: options.title ?? "Are you sure?",
        message: options.message ?? "This action cannot be undone.",
        confirmText: options.confirmText ?? "Confirm",
        cancelText: options.cancelText ?? "Cancel",
        variant: options.variant ?? "danger",
      };

      return new Promise((resolve) => {
        confirmResolver = resolve;
      });
    },

    resolveConfirm(value) {
      const resolve = confirmResolver;

      confirmResolver = null;
      this.confirmDialog.visible = false;

      if (resolve) {
        resolve(value);
      }
    },
  },
});
