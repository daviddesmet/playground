<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md4>
      <v-card class="elevation-8">
        <v-toolbar color="secondary" flat dark>
          <v-toolbar-title>Recovery Codes</v-toolbar-title>
        </v-toolbar>
        <v-form>
          <v-card-text>
            <v-layout align-center justify-center>
              <v-icon color="orange" x-large>security</v-icon>
            </v-layout>
            <v-container grid-list-md text-xs-center>
              <v-layout row wrap>
                <v-flex v-for="code in codes" :key="code" xs6>
                  <div class="caption code">{{ code | format }}</div>
                </v-flex>
                <div v-show="codes.count <= 0" class="mt-3 subheading text-xs-center">No backup codes</div>
              </v-layout>
            </v-container>
          </v-card-text>
          <v-alert type="info" value="true">
            Remember to save your backup codes
          </v-alert>
          <v-alert type="error" :value="errors">
            {{ errors }}
          </v-alert>
          <v-card-actions v-show="!isFirstTime">
            <v-btn depressed color="orange" @click="submit">Get New Codes</v-btn>
            <v-spacer />
            <v-btn outlined text color="primary" to="/account">Close</v-btn>
          </v-card-actions>
          <v-card-actions v-show="isFirstTime">
            <v-btn depressed block color="primary" to="/account">Close</v-btn>
          </v-card-actions>
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
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { AUTHENTICATOR_CODES_REQUEST } from "@/store/modules/user/actions.type";

@Component({
  filters: {
    format(value: string) {
      return value ? value.slice(0, 4) + "-" + value.slice(4) : "";
    }
  }
})
export default class AuthenticatorCodes extends Vue {
  private isBusy = false;
  private errors: string | null = null;
  private isFirstTime = false;
  private codes: string[] = [];

  private clearErrors() {
    this.errors = null;
  }

  private created() {
    this.submit();
  }

  private submit() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`user/${AUTHENTICATOR_CODES_REQUEST}`)
      .then((result: any) => {
        // console.log(result);
        this.codes = result;
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

<style lang="scss" scoped>
@import url(https://fonts.googleapis.com/css?family=Inconsolata);

.code {
  font-family: "Inconsolata";
  font-size: 18px !important;
}
</style>
