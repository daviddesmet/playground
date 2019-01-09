import { Module } from "vuex";
import { getters } from "./getters";
import { actions } from "./actions";
import { mutations } from "./mutations";
import { CoreState } from "../../types";
import { UserState } from "./types";
import { Profile } from "@/models/profile.interface";

export const state: UserState = {
  profile: {} as Profile,
  status: ""
};

const namespaced: boolean = true;

export const user: Module<UserState, CoreState> = {
  namespaced,
  state,
  getters,
  actions,
  mutations
};
