import { GetterTree } from "vuex";
import { CoreState } from "../../types";
import { UserState } from "./types";

export const getters: GetterTree<UserState, CoreState> = {
  profile: (userState: UserState) => userState.profile
};
