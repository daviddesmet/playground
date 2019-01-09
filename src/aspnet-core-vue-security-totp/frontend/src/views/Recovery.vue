<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex xs12 sm8 md4>
        <v-card class="elevation-12">
          <v-toolbar dark color="primary">
            <v-toolbar-title>2-Step Recovery</v-toolbar-title>
          </v-toolbar>
          <v-form @submit.prevent>
            <v-card-text>
              <v-layout align-center justify-center>
                <v-icon x-large color="amber">lock</v-icon>
              </v-layout>
              <v-spacer></v-spacer>
              <v-text-field prepend-icon="verified_user" name="code" label="Recovery Code" type="text" v-model="code" required autofocus @keyup.enter="submit"></v-text-field>
            </v-card-text>
            <v-alert type="error" :value="errors">
              {{ errors }}
            </v-alert>
            <v-btn primary color="green" large block @click="submit">Verify</v-btn>
          </v-form>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { VerificationRecovery } from "@/models/verification.recovery.interface";
import { AUTH_RECOVERY_REQUEST } from "@/store/modules/auth/actions.type";

@Component
export default class Recovery extends Vue {
  private errors: string | null = "";
  private isBusy: boolean = false;
  private code?: string | null = null;

  private clearErrors() {
    this.errors = null;
  }

  private submit() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`auth/${AUTH_RECOVERY_REQUEST}`, {
        recoveryCode: this.code
      } as VerificationRecovery)
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
