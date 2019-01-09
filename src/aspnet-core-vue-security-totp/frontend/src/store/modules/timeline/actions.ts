import { ActionTree } from "vuex";
import { TimelineRecord } from "@/models/timeline.interface";
import { CoreState } from "../../types";
import { TimelineState } from "./types";
import { UPDATE_TIMELINE } from "./actions.type";

export const actions: ActionTree<TimelineState, CoreState> = {
  [UPDATE_TIMELINE]: ({ commit, dispatch }: { commit: any; dispatch: any }, item: TimelineRecord) => {
    commit(UPDATE_TIMELINE, item);
  }
};
