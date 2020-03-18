<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md4>
      <v-card class="elevation-8">
        <v-toolbar color="secondary" flat dark>
          <v-toolbar-title>Password Change</v-toolbar-title>
        </v-toolbar>
        <v-form ref="form" v-model="valid" lazy-validation @submit.prevent>
          <v-card-text>
            <v-layout align-center justify-center>
              <v-icon x-large color="amber">lock</v-icon>
            </v-layout>
            <v-spacer></v-spacer>
            <v-text-field
              prepend-icon="lock_open"
              name="OldPassword"
              label="Old Password"
              type="password"
              v-model="credentials.oldPassword"
              :rules="oldPasswordRules"
              required
              @keyup="clearErrors"
            ></v-text-field>
            <v-text-field
              prepend-icon="lock"
              name="NewPassword"
              label="New Password"
              type="password"
              v-model="credentials.newPassword"
              :rules="newPasswordRules"
              required
              @keyup="clearErrors"
            ></v-text-field>
            <v-text-field
              prepend-icon="done"
              name="ConfirmPassword"
              label="Confirm Password"
              type="password"
              v-model="credentials.confirmPassword"
              :rules="confirmPasswordRules"
              required
              @keyup="clearErrors"
            ></v-text-field>
          </v-card-text>
          <v-alert type="error" :value="errors">
            {{ errors }}
          </v-alert>
          <v-btn depressed large block color="primary" @click="submit">Confirm</v-btn>
        </v-form>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { CredentialsChangePassword } from "@/models/credentials.newpassword.interface";
import { USER_PASSWORD_CHANGE } from "@/store/modules/user/actions.type";

@Component
export default class ChangePassword extends Vue {
  private valid = false;
  private oldPasswordRules: unknown = [(v: string) => !!v || "Old Password is required"];
  private newPasswordRules: unknown = [(v: string) => !!v || "New Password is required"];
  private confirmPasswordRules: unknown = [(v: string) => !!v || "Confirm Password is required"];
  private errors: string | null = null;
  private isBusy = false;
  private credentials = {} as CredentialsChangePassword;

  private clearErrors() {
    this.errors = null;
  }

  private submit() {
    if ((this.$refs.form as Vue & { validate: () => boolean }).validate()) {
      this.isBusy = true;
      this.clearErrors();
      this.$store
        .dispatch(`user/${USER_PASSWORD_CHANGE}`, this.credentials)
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
}
</script>
