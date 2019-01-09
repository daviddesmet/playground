import { MutationTree } from "vuex";
import { UserState } from "./types";
import {
  USER_REQUEST,
  USER_SUCCESS,
  USER_ERROR,
  AUTHENTICATOR_ENABLE_SUCCESS,
  AUTHENTICATOR_ENABLE_ERROR,
  AUTHENTICATOR_DISABLE_SUCCESS,
  AUTHENTICATOR_DISABLE_ERROR,
  AUTHENTICATOR_RESET_SUCCESS,
  AUTHENTICATOR_RESET_ERROR
} from "./actions.type";
import { Profile } from "@/models/profile.interface";

export const mutations: MutationTree<UserState> = {
  [USER_REQUEST]: (userState: UserState) => {
    userState.status = "profile_request";
  },
  [USER_SUCCESS]: (userState: UserState, userResp: Profile) => {
    userState.profile = userResp;
    userState.status = "profile_success";
    // Vue.set(userState, "profile", userResp);
  },
  [USER_ERROR]: (userState: UserState) => {
    userState.status = "profile_error";
  },
  [AUTHENTICATOR_ENABLE_SUCCESS]: (userState: UserState) => {
    if (userState.profile) {
      userState.profile.twoFactorEnabled = true;
    }

    userState.status = "profile_2fa_enabled";
  },
  [AUTHENTICATOR_ENABLE_ERROR]: (userState: UserState) => {
    userState.status = "profile_2fa_enable_error";
  },
  [AUTHENTICATOR_DISABLE_SUCCESS]: (userState: UserState) => {
    if (userState.profile) {
      userState.profile.twoFactorEnabled = false;
    }

    userState.status = "profile_2fa_disabled";
  },
  [AUTHENTICATOR_DISABLE_ERROR]: (userState: UserState) => {
    userState.status = "profile_2fa_disable_error";
  },
  [AUTHENTICATOR_RESET_SUCCESS]: (userState: UserState) => {
    if (userState.profile) {
      userState.profile.twoFactorEnabled = false;
    }

    userState.status = "profile_2fa_reset";
  },
  [AUTHENTICATOR_RESET_ERROR]: (userState: UserState) => {
    userState.status = "profile_2fa_reset_error";
  }
};
