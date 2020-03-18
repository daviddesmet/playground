<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md4>
      <v-card class="elevation-8">
        <v-toolbar color="secondary" flat dark>
          <v-toolbar-title>Account Info</v-toolbar-title>
        </v-toolbar>
        <v-form>
          <v-card-text>
            <v-text-field prepend-icon="person" name="name" label="Name" type="text" v-model="profile.name" disabled readonly></v-text-field>
            <v-text-field prepend-icon="email" name="email" label="Email" type="email" v-model="profile.email" disabled readonly></v-text-field>
            <v-text-field prepend-icon="place" name="location" label="Location" type="text" v-model="profile.location" disabled readonly></v-text-field>
          </v-card-text>
          <v-alert type="error" :value="errors">
            {{ errors }}
          </v-alert>
          <v-card-actions v-show="!profile.twoFactorEnabled">
            <v-btn color="orange" to="/account/changepassword" large block flat>Change Password</v-btn>
          </v-card-actions>
          <v-card-actions v-show="profile.twoFactorEnabled">
            <v-btn depressed large block text color="orange" to="/account/changepassword">Change Password</v-btn>
            <v-btn depressed large block text color="green" to="/account/2fa/codes">2-Step Backup Codes</v-btn>
          </v-card-actions>
          <v-btn v-show="profile.twoFactorEnabled" depressed large block color="red" @click="disableTwoFactor">Disable 2-Step Verification</v-btn>
          <v-btn v-show="!profile.twoFactorEnabled" depressed large block color="green" to="/account/2fa/setup">Enable 2-Step Verification</v-btn>
        </v-form>
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
import { mapGetters } from "vuex";
import { USER_REQUEST, AUTHENTICATOR_DISABLE_REQUEST } from "../store/modules/user/actions.type";

@Component({
  computed: mapGetters({
    profile: "user/profile"
  })
})
export default class Account extends Vue {
  private isBusy = false;
  private errors: string | null = "";

  private clearErrors() {
    this.errors = null;
  }

  private created() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`user/${USER_REQUEST}`)
      .catch(err => {
        console.log(err);
        this.errors = err;
      })
      .then(() => {
        this.isBusy = false;
      });
  }

  private disableTwoFactor() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`user/${AUTHENTICATOR_DISABLE_REQUEST}`)
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
