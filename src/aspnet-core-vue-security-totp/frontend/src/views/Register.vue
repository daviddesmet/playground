<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-flex xs12 sm8 md4>
        <v-card class="elevation-12">
          <v-toolbar dark color="primary">
            <v-toolbar-title>Register</v-toolbar-title>
          </v-toolbar>
          <!-- <v-img alt="Vue logo" src="../assets/logo.png" /> -->
          <v-card-text>
            <v-form ref="form" v-model="valid" lazy-validation>
              <v-text-field prepend-icon="person" name="name" label="Name" type="text" v-model="user.name" :rules="nameRules" required></v-text-field>
              <v-text-field prepend-icon="place" name="location" label="Location" type="text" v-model="user.location"></v-text-field>
              <v-text-field prepend-icon="email" name="email" label="Email" type="email" v-model="user.email" :rules="emailRules" required></v-text-field>
              <v-text-field prepend-icon="lock" name="password" label="Password" type="password" v-model="user.password" :rules="passwordRules" required></v-text-field>
            </v-form>
          </v-card-text>
          <v-alert color="error" icon="warning" value="true" v-show="errors">
            {{ errors }}
          </v-alert>
          <v-card-actions>
            <!-- <v-spacer></v-spacer> -->
            <v-btn block large color="orange" @click="submit" :disabled="!valid">Submit</v-btn>
          </v-card-actions>
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
import { UserRegistration } from "@/models/user.registration.interface";
import { accountService } from "@/services/account.service";
import { finalize } from "rxjs/operators";

@Component
export default class Register extends Vue {
  private valid: boolean = false;
  private nameRules: any = [(v: string) => !!v || "Name is required"];
  private emailRules: any = [
    (v: string) => !!v || "E-mail is required",
    (v: string) => /.+@.+/.test(v) || "E-mail must be valid"
  ];
  private passwordRules: any = [
    (v: string) => !!v || "Password is required",
    (v: string) =>
      (!!v && v.length >= 6) || "Password must be greater than 6 characters"
  ];

  private errors: string | null = "";
  private isBusy: boolean = false;
  private user = {} as UserRegistration;

  private clearError() {
    this.errors = null;
  }

  private submit() {
    if ((this.$refs.form as Vue & { validate: () => boolean }).validate()) {
      this.isBusy = true;
      this.clearError();
      accountService
        .register(this.user)
        .pipe(finalize(() => (this.isBusy = false)))
        .subscribe(
          (result: any) => {
            this.$router.push({
              name: "login",
              query: { new: "true", email: this.user.email }
            });
          },
          (err: any) => {
            console.log(err);
            this.errors = err;
          }
        );
    }
  }
}
</script>
