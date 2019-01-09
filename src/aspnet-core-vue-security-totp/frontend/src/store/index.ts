import Vue from "vue";
import Vuex, { StoreOptions } from "vuex";
import { getters } from "./getters";
import { mutations } from "./mutations";
import { actions } from "./actions";
import { auth } from "./modules/auth";
import { user } from "./modules/user";
import { timeline } from "./modules/timeline";
import { CoreState } from "./types";

Vue.use(Vuex);

const state: CoreState = {
  version: "1.0.0"
};

const store: StoreOptions<CoreState> = {
  state,
  getters,
  mutations,
  actions,
  modules: {
    auth,
    user,
    timeline
  }
};

export default new Vuex.Store<CoreState>(store);
