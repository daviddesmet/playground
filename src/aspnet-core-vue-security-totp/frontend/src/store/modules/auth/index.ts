import { Module } from "vuex";
import { getters } from "./getters";
import { actions } from "./actions";
import { mutations } from "./mutations";
import { CoreState } from "../../types";
import { AuthState } from "./types";
import { getToken } from "@/services/jwt.service";

export const state: AuthState = {
  token: getToken() || "",
  status: ""
};

const namespaced = true;

export const auth: Module<AuthState, CoreState> = {
  namespaced,
  state,
  getters,
  actions,
  mutations
};
