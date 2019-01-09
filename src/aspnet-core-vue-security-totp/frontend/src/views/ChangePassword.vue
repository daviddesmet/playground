<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex xs12 sm8 md4>
        <v-card class="elevation-12">
          <v-toolbar dark color="primary">
            <v-toolbar-title>Password Change</v-toolbar-title>
          </v-toolbar>
          <v-form ref="form" v-model="valid" lazy-validation @submit.prevent>
            <v-card-text>
              <v-layout align-center justify-center>
                <v-icon x-large color="amber">lock</v-icon>
              </v-layout>
              <v-spacer></v-spacer>
              <v-text-field prepend-icon="lock_open" name="OldPassword" label="Old Password" type="password" v-model="credentials.oldPassword" :rules="oldPasswordRules" required @keyup="clearErrors"></v-text-field>
              <v-text-field prepend-icon="lock" name="NewPassword" label="New Password" type="password" v-model="credentials.newPassword" :rules="newPasswordRules" required @keyup="clearErrors"></v-text-field>
              <v-text-field prepend-icon="done" name="ConfirmPassword" label="Confirm Password" type="password" v-model="credentials.confirmPassword" :rules="confirmPasswordRules" required @keyup="clearErrors"></v-text-field>
            </v-card-text>
            <v-alert type="error" :value="errors">
              {{ errors }}
            </v-alert>
            <v-btn primary color="green" large block @click="submit">Confirm</v-btn>
          </v-form>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { CredentialsChangePassword } from "@/models/credentials.newpassword.interface";
import { USER_PASSWORD_CHANGE } from "@/store/modules/user/actions.type";

@Component
export default class ChangePassword extends Vue {
  private valid: boolean = false;
  private oldPasswordRules: any = [
    (v: string) => !!v || "Old Password is required"
  ];
  private newPasswordRules: any = [
    (v: string) => !!v || "New Password is required"
  ];
  private confirmPasswordRules: any = [
    (v: string) => !!v || "Confirm Password is required"
  ];
  private errors: string | null = "";
  private isBusy: boolean = false;
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
