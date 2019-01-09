import { GetterTree } from "vuex";
import { CoreState } from "../../types";
import { TimelineState } from "./types";

export const getters: GetterTree<TimelineState, CoreState> = {
  timeline: (timelineState: TimelineState) => timelineState.timeline
};
