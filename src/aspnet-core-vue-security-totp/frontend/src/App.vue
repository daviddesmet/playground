<template>
  <v-app dark>
    <v-app-bar color="secondary" absolute dark>
      <!-- <div class="d-flex align-center">
        <v-img alt="Vuetify Logo" class="shrink mr-2" contain src="https://cdn.vuetifyjs.com/images/logos/vuetify-logo-dark.png" transition="scale-transition" width="40" />
        <v-img alt="Vuetify Name" class="shrink mt-1 hidden-sm-and-down" contain min-width="100" src="https://cdn.vuetifyjs.com/images/logos/vuetify-name-dark.png" width="100" />
      </div>
      <v-spacer></v-spacer>
      <v-btn href="https://github.com/vuetifyjs/vuetify/releases/latest" target="_blank" text>
        <span class="mr-2">Latest Release</span>
        <v-icon>mdi-open-in-new</v-icon>
      </v-btn> -->
      <router-link class="toolbar-title" to="/">
        <v-toolbar-title class="headline text-uppercase" to="/">
          <span>Web</span>
          <span class="font-weight-light">Security</span>
        </v-toolbar-title>
      </router-link>
      <v-spacer></v-spacer>
      <v-toolbar-items class="hidden-sm-and-down">
        <v-btn text to="/timeline" v-if="isLoggedIn">
          <span>Timeline</span>
        </v-btn>
        <v-btn text to="/account" v-if="isLoggedIn">
          <span>Account</span>
        </v-btn>
        <v-btn text to="/login" v-if="!isLoggedIn">
          <span>Login</span>
        </v-btn>
        <v-btn text to="/logout" v-if="isLoggedIn">
          <span>Logout</span>
        </v-btn>
      </v-toolbar-items>
    </v-app-bar>

    <!-- Sizes your content based upon application components -->
    <v-content>
      <!-- Provides the application the proper gutter -->
      <v-container fill-height fluid>
        <router-view></router-view>
      </v-container>
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
//import { State, Action, Getter } from "vuex-class";
//import { AuthState } from "./store/modules/auth/types";

//const namespace = "auth";

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
