<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md4>
      <v-card class="elevation-8">
        <v-toolbar color="secondary" flat dark>
          <v-toolbar-title>Login</v-toolbar-title>
        </v-toolbar>
        <!-- <v-img alt="Vue logo" src="../assets/logo.png" /> -->
        <!-- <v-alert color="error" icon="warning" value="true" v-show="errors">
            {{ errors }}
          </v-alert> -->
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-text-field
              prepend-icon="person"
              name="email"
              label="Email"
              type="email"
              v-model="credentials.userName"
              :rules="emailRules"
              required
              autofocus
              @keyup="clearErrors"
              @keyup.enter="submit"
            ></v-text-field>
            <v-text-field
              prepend-icon="lock"
              name="Password"
              label="Password"
              type="password"
              v-model="credentials.password"
              :rules="passwordRules"
              required
              @keyup="clearErrors"
              @keyup.enter="submit"
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-alert type="error" :value="errors">
          {{ errors }}
        </v-alert>
        <v-spacer />
        <v-card-actions>
          <v-btn outlined color="orange" to="/register">Register</v-btn>
          <v-btn depressed color="primary" @click="submit" :disabled="!valid">Login</v-btn>
        </v-card-actions>
      </v-card>
      <!-- <v-dialog v-model="isBusy" hide-overlay persistent width="300">
          <v-card color="primary" dark>
            <v-card-text>
              Please stand by
              <v-progress-linear indeterminate color="white" class="mb-0"></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-dialog> -->
    </v-flex>
  </v-layout>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { Credentials } from "../models/credentials.interface";
import { AUTH_REQUEST } from "../store/modules/auth/actions.type";

@Component
export default class Login extends Vue {
  private valid = false;
  private emailRules: unknown = [(v: string) => !!v || "E-mail is required"];
  private passwordRules: unknown = [(v: string) => !!v || "Password is required"];
  private errors: string | null = null;
  private isBusy = false;
  private credentials = {} as Credentials;

  private created() {
    if (this.$route.query.new) {
      this.credentials.userName = this.$route.query.email as string;
    }
  }

  private clearErrors() {
    this.errors = null;
  }

  private submit() {
    if ((this.$refs.form as Vue & { validate: () => boolean }).validate()) {
      this.isBusy = true;
      this.clearErrors();
      this.$store
        .dispatch(`auth/${AUTH_REQUEST}`, this.credentials)
        .then((result: unknown) => {
          if (result) {
            this.$router.push("/account");
          } else {
            this.$router.push("/account/authenticate");
          }
        })
        .catch(err => {
          console.log(err);
          this.errors = err;
        })
        .then(() => {
          this.isBusy = false;
        });
    }
  }
}
</script>
