import Vue from "vue";
import "./plugins/vuetify";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import axios from "axios";
// import VueCookies from "vue-cookies";

import { getToken, parseToken } from "@/services/jwt.service";
// import { AUTH_REFRESH_TOKEN, AUTH_LOGOUT } from "@/store/modules/auth/actions.type";

import "../node_modules/nprogress/nprogress.css";

// Enable devtools
Vue.config.devtools = process.env.NODE_ENV !== "production";

// Don't warn about using the dev version of Vue in development
Vue.config.productionTip = process.env.NODE_ENV === "production";

// Vue.use(VueCookies);

new Vue({
  store,
  router,
  render: h => h(App)
}).$mount("#app");

const getAuthToken = (): string => {
  const tfaToken = getToken("tfa_token");

  if (tfaToken) {
    return tfaToken;
  }

  const authToken = store.getters["auth/authToken"];

  /* TODO: Implement token refresh (do not use below code, it's not tested)
  const exp = parseToken(authToken).exp as number;
  const max = ((Date.now() / 1000).toFixed(0) as unknown) as number;
  if (exp - 240 <= max) {
    store.dispatch(`auth/${AUTH_REFRESH_TOKEN}`)
      .then((result: any) => {
        return store.getters["auth/authToken"];
      })
      .catch(err => {
        store.dispatch(`auth/${AUTH_LOGOUT}`);
      });
  }
  */

  return authToken;
};

axios.interceptors.request.use(
  (config: any) => {
    config.headers.Authorization = `Bearer ${getAuthToken()}`;
    return config;
  },
  (err: any) => {
    return Promise.reject(err);
  }
);
