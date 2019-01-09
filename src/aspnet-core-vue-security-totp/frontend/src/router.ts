import Vue from "vue";
import Router from "vue-router";
import NProgress from "nprogress/nprogress";
import Home from "./views/Home.vue";
import store from "./store";
import { AUTH_LOGOUT } from "./store/modules/auth/actions.type";

NProgress.configure({ showSpinner: false });

Vue.use(Router);

const router = new Router({
  // Use the HTML5 history API (i.e. normal-looking routes)
  // instead of routes with hashes (e.g. example.com/#/about).
  // This may require some server configuration in production:
  // https://router.vuejs.org/en/essentials/history-mode.html#example-server-configurations
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "home",
      component: Home
    },
    {
      path: "/about",
      name: "about",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ "./views/About.vue")
    },
    {
      path: "/account",
      name: "account",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => lazyLoadView(import(/* webpackChunkName: "account" */ "@/views/Account.vue")),
      meta: {
        authRequired: true
      }
    },
    {
      path: "/timeline",
      name: "timeline",
      component: () => lazyLoadView(import(/* webpackChunkName: "timeline" */ "@/views/AccountTimeline.vue")),
      meta: {
        authRequired: true
      }
    },
    {
      path: "/account/changepassword",
      name: "changepassword",
      component: () => lazyLoadView(import(/* webpackChunkName: "changepassword" */ "@/views/ChangePassword.vue")),
      meta: {
        authRequired: true
      }
    },
    {
      path: "/account/2fa/setup",
      name: "2fa",
      component: () =>
        lazyLoadView(import(/* webpackChunkName: "authenticatorsetup" */ "@/views/AuthenticatorSetup.vue")), // tslint:disable-line
      meta: {
        authRequired: true
      }
    },
    {
      path: "/account/2fa/codes",
      name: "2fa-codes",
      component: () =>
        lazyLoadView(import(/* webpackChunkName: "authenticatorcodes" */ "@/views/AuthenticatorCodes.vue")), // tslint:disable-line
      meta: {
        authRequired: true
      }
    },
    {
      path: "/account/authenticate",
      name: "authenticate",
      component: () => lazyLoadView(import(/* webpackChunkName: "authenticate" */ "@/views/Authenticate.vue")),
      beforeEnter: (to: any, from: any, next: any) => {
        store.getters["auth/authStatus"] === "twofactor" ? next() : next({ path: "/login" });
      }
    },
    {
      path: "/account/authenticate/recovery",
      name: "recovery",
      component: () => lazyLoadView(import(/* webpackChunkName: "recovery" */ "@/views/Recovery.vue")),
      beforeEnter: (to: any, from: any, next: any) => {
        store.getters["auth/authStatus"] === "twofactor" ? next() : next({ path: "/login" });
      }
    },
    {
      path: "/login",
      name: "login",
      component: () => lazyLoadView(import(/* webpackChunkName: "login" */ "@/views/Login.vue"))
    },
    {
      path: "/register",
      name: "register",
      component: () => lazyLoadView(import(/* webpackChunkName: "register" */ "@/views/Register.vue"))
    },
    {
      path: "/logout",
      name: "logout",
      meta: {
        authRequired: true
      },
      async beforeEnter(to: any, from: any, next: any) {
        await store.dispatch(`auth/${AUTH_LOGOUT}`).then((result: any) => {
          const authRequiredOnPreviousRoute = from.matched.some((route: any) => route.meta.authRequired);
          // Navigate back to previous page or home as a fallback
          next(authRequiredOnPreviousRoute ? { name: "home" } : { ...from });
        });
      }
    },
    // Redirect any unmatched routes to the 404 page. This may require some server configuration to work in production:
    // https://router.vuejs.org/en/essentials/history-mode.html#example-server-configurations
    {
      path: "*",
      redirect: "404"
    }
  ]
});

// After navigation is confirmed, but before resolving...
router.beforeResolve((routeTo, routeFrom, next) => {
  // If this isn't an initial page load...
  if (routeFrom.name) {
    // Start the route progress bar.
    NProgress.start();
  }
  next();
});

router.beforeEach((to: any, from: any, next: any) => {
  if (to.matched.some((record: any) => record.meta.authRequired)) {
    // store.state.loggedIn ? next() : next({ path: "/login", query: { redirect: to.path } });
    store.getters["auth/isAuthenticated"] ? next() : next({ path: "/login", query: { redirect: to.path } });
  } else {
    next();
  }
});

// Lazy-loads view components, but with better UX. A loading view
// will be used if the component takes a while to load, falling
// back to a timeout view in case the page fails to load. You can
// use this component to lazy-load a route with:
//
// component: () => lazyLoadView(import("@views/my-view"))
//
// NOTE: Components loaded with this strategy DO NOT have access
// to in-component guards, such as beforeRouteEnter,
// beforeRouteUpdate, and beforeRouteLeave. You must either use
// route-level guards instead or lazy-load the component directly:
//
// component: () => import("@views/my-view")
//
function lazyLoadView(AsyncView: any) {
  const AsyncHandler = () => ({
    component: AsyncView,
    // A component to use while the component is loading.
    loading: require("@/views/Loading").default,
    // A fallback component in case the timeout is exceeded when loading the component.
    error: require("@/views/Timeout").default,
    // Delay before showing the loading component.
    // Default: 200 (milliseconds).
    delay: 400,
    // Time before giving up trying to load the component.
    // Default: Infinity (milliseconds).
    timeout: 10000
  });

  return Promise.resolve({
    functional: true,
    render(h: any, { data, children }: { data: any; children: any }) {
      // Transparently pass any props or children
      // to the view component.
      return h(AsyncHandler, data, children);
    }
  });
}

// When each route is finished evaluating...
router.afterEach((routeTo, routeFrom) => {
  // Complete the animation of the route progress bar.
  NProgress.done();
});

export default router;
