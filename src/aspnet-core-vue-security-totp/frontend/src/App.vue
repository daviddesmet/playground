<template>
  <v-app dark>
    <v-toolbar app>
      <router-link class="toolbar-title" to="/">
        <v-toolbar-title class="headline text-uppercase" to="/">
          <span>Web</span>
          <span class="font-weight-light">Security</span>
        </v-toolbar-title>
      </router-link>
      <v-spacer></v-spacer>
      <v-toolbar-items class="hidden-sm-and-down">
        <v-btn flat to="/timeline" v-if="isLoggedIn">
          <span>Timeline</span>
        </v-btn>
        <v-btn flat to="/account" v-if="isLoggedIn">
          <span>Account</span>
        </v-btn>
        <v-btn flat to="/login" v-if="!isLoggedIn">
          <span>Login</span>
        </v-btn>
        <v-btn flat to="/logout" v-if="isLoggedIn">
          <span>Logout</span>
        </v-btn>
      </v-toolbar-items>
    </v-toolbar>

    <v-content>
      <router-view></router-view>
    </v-content>
    <v-footer class="pa-3">
      <v-spacer></v-spacer>
      <div>&copy; {{ new Date().getFullYear() }} — <strong>David De Smet</strong></div>
    </v-footer>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { State, Action, Getter } from "vuex-class";
import { AuthState } from "./store/modules/auth/types";

const namespace: string = "auth";

@Component
export default class App extends Vue {
  // @State("token", { namespace }) private token!: string; // @State(namespace) private auth!: AuthState;

  // computed variable based on user's token
  get isLoggedIn() {
    // return this.token; // using Sate
    return this.$store.getters["auth/isAuthenticated"];
  }
}
</script>

<style lang="scss">
.toolbar-title {
  color: inherit;
  text-decoration: inherit;
}
</style>
