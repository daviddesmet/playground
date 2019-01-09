import { Module } from "vuex";
import { getters } from "./getters";
import { actions } from "./actions";
import { mutations } from "./mutations";
import { CoreState } from "../../types";
import { TimelineState } from "./types";
import { TimelineRecord } from "@/models/timeline.interface";

export const state: TimelineState = {
  status: "",
  timeline: [] as TimelineRecord[]
};

const namespaced: boolean = true;

export const timeline: Module<TimelineState, CoreState> = {
  namespaced,
  state,
  getters,
  actions,
  mutations
};
