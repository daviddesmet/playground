<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex xs12 sm8 md4>
        <v-card class="elevation-12">
          <v-toolbar dark color="primary">
            <v-toolbar-title>2-Step Verification</v-toolbar-title>
          </v-toolbar>
          <v-form @submit.prevent>
            <v-card-text>
              <v-layout align-center justify-center>
                <v-icon x-large color="amber">fingerprint</v-icon>
              </v-layout>
              <v-spacer></v-spacer>
              <v-text-field prepend-icon="verified_user" name="code" label="Verification Code" type="text" v-model="code" required autofocus @keyup.enter="submit"></v-text-field>
              <v-tooltip top>
                <v-switch slot="activator" label="Remember this device" color="info" v-model="rememberDevice"></v-switch>
                <span>Up to 30 days</span>
              </v-tooltip>
            </v-card-text>
            <v-alert type="error" :value="errors">
              {{ errors }}
            </v-alert>
            <v-card-actions>
              <v-btn color="orange" to="/account/authenticate/recovery" large block flat>Use Recovery Code</v-btn>
            </v-card-actions>
            <v-btn primary color="green" large block @click="submit">Done</v-btn>
          </v-form>
        </v-card>
        <v-dialog v-model="isBusy" hide-overlay persistent width="300">
          <v-card color="primary" dark>
            <v-card-text>
              Please stand by
              <v-progress-linear indeterminate color="white" class="mb-0"></v-progress-linear>
            </v-card-text>
          </v-card>
        </v-dialog>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { Verification } from "@/models/verification.interface";
import { AUTH_VERIFICATION_REQUEST } from "@/store/modules/auth/actions.type";

@Component
export default class Authenticate extends Vue {
  private errors: string | null = "";
  private isBusy: boolean = false;
  private code?: string | null = null;
  private rememberDevice: boolean = false;

  private clearErrors() {
    this.errors = null;
  }

  private submit() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`auth/${AUTH_VERIFICATION_REQUEST}`, {
        code: this.code,
        rememberDevice: this.rememberDevice
      } as Verification)
      .then((result: any) => {
        this.$router.push("/account");
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
</script>
