import { MutationTree } from "vuex";
import { AuthState } from "./types";
import {
  AUTH_REQUEST,
  AUTH_TWOFACTOR,
  AUTH_SUCCESS,
  AUTH_ERROR,
  AUTH_LOGOUT
} from "./actions.type";

export const mutations: MutationTree<AuthState> = {
  [AUTH_REQUEST]: (authState: AuthState) => {
    authState.status = "init";
  },
  [AUTH_TWOFACTOR]: (authState: AuthState) => {
    authState.status = "twofactor";
  },
  [AUTH_SUCCESS]: (authState: AuthState, authToken: string) => {
    authState.status = "succeeded";
    authState.token = authToken;
  },
  [AUTH_ERROR]: (authState: AuthState) => {
    authState.status = "error";
  },
  [AUTH_LOGOUT]: (authState: AuthState) => {
    authState.token = "";
  }
};
