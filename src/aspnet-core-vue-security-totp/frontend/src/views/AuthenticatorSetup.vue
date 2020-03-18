<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md4>
      <v-card class="elevation-8">
        <v-toolbar color="secondary" flat dark>
          <v-toolbar-title>Enable Authenticator</v-toolbar-title>
        </v-toolbar>
        <v-form @submit.prevent>
          <v-card-text>
            <v-layout row align-center justify-center>
              <v-card color="white">
                <v-layout justify-center>
                  <QRCanvas ref="qr" :value="options" height="168" width="168" class="ma-3"></QRCanvas>
                </v-layout>
              </v-card>
            </v-layout>
            <v-layout align-center justify-center>
              <div class="ma-3 caption">{{ authenticator.sharedKey }}</div>
            </v-layout>
            <v-spacer></v-spacer>
            <v-text-field
              prepend-icon="verified_user"
              name="code"
              label="Verification Code"
              type="text"
              v-model="authenticator.code"
              @keyup.enter="submit"
              autofocus
            ></v-text-field>
          </v-card-text>
          <v-alert color="error" icon="warning" value="true" v-show="errors">
            {{ errors }}
          </v-alert>
          <v-btn depressed large block color="primary" @click="submit">Verify</v-btn>
        </v-form>
      </v-card>
      <v-dialog v-model="isBusy" hide-overlay persistent width="300">
        <v-card color="primary" dark>
          <v-card-text>
            Obtaining authenticator's QR code...
            <v-progress-linear indeterminate color="white" class="mb-0"></v-progress-linear>
          </v-card-text>
        </v-card>
      </v-dialog>
    </v-flex>
  </v-layout>
</template>

<script lang="ts">
import { Vue, Component, Watch } from "vue-property-decorator";
import { Authenticator } from "@/models/authenticator.interface";
import { AUTHENTICATOR_REQUEST, AUTHENTICATOR_ENABLE_REQUEST } from "@/store/modules/user/actions.type";

// import Qrcanvas from "qrcanvas-vue";
// const QRCanvas = () => import("qrcanvas-vue").then(m => m.default);
import { QRCanvas } from "qrcanvas-vue";

@Component({
  components: {
    QRCanvas
  }
})
export default class AuthenticatorSetup extends Vue {
  private isBusy = false;
  private errors: string | null = "";
  private options = {} as any;
  private authenticator = {} as Authenticator;

  private clearErrors() {
    this.errors = null;
  }

  private created() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`user/${AUTHENTICATOR_REQUEST}`)
      .then((result: any) => {
        // console.log(result);
        this.authenticator = result;
        this.options = {
          // cellSize: 8,
          data: this.authenticator.authenticatorUri
        };
        // Not working as per documentation, using below hack instead...
        // this.options = Object.assign({}, this.options, {
        //   data: this.authenticator.authenticatorUri
        // });
        // HACK: Update directly since options is not updated when using a computed property...
        (this.$refs.qr as Vue & { update: (opts: any) => void }).update(this.options);
      })
      .catch(err => {
        console.log(err);
        this.errors = err;
      })
      .then(() => {
        this.isBusy = false;
      });
  }

  private submit() {
    this.isBusy = true;
    this.clearErrors();
    this.$store
      .dispatch(`user/${AUTHENTICATOR_ENABLE_REQUEST}`, this.authenticator)
      .then((result: any) => {
        this.$router.push("/account/2fa/codes");
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
