<template>
  <v-container fill-height>
    <v-layout align-center justify-center>
      <v-timeline>
        <v-timeline-item v-for="(time, i) in timeline" :key="i" :color="time.color" small>
          <span slot="opposite" :class="`headline font-weight-bold ${time.color}--text`" v-text="time.title"></span>
          <div class="py-3">
            <h2 :class="`headline font-weight-light mb-3 ${time.color}--text`">{{ time.date }}</h2>
            <div>
              {{ time.message }}
            </div>
          </div>
        </v-timeline-item>
      </v-timeline>
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import { mapGetters } from "vuex";

@Component({
  computed: mapGetters({
    timeline: "timeline/timeline"
  })
})
export default class AccountTimeline extends Vue {
  private isBusy: boolean = false;
  private errors: string | null = "";
}
</script>
