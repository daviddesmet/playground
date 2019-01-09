<template>
  <div class="vld-parent">
    <loading :active="true" loader="dots"></loading>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";

import Loading from "vue-loading-overlay";
import "vue-loading-overlay/dist/vue-loading.css";

@Component({
  components: {
    Loading
  }
})
export default class LoadingPage extends Vue {}
</script>
