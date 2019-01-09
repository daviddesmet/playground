import { MutationTree } from "vuex";
import { TimelineRecord } from "@/models/timeline.interface";
import { TimelineState } from "./types";
import { UPDATE_TIMELINE } from "./actions.type";

const now = (): string => {
  const options = {
    weekday: "short",
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "numeric",
    minute: "numeric",
    second: "numeric"
  };
  return new Date(Date.now()).toLocaleString("en-GB", options);
};

export const mutations: MutationTree<TimelineState> = {
  [UPDATE_TIMELINE]: (timelineState: TimelineState, item: TimelineRecord) => {
    timelineState.status = item.message;
    timelineState.timeline.unshift({
      date: item.date || now(),
      title: item.title,
      message: item.message,
      color: item.color
    });
  }
};
