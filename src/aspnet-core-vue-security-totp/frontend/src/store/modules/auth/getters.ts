import { GetterTree } from "vuex";
import { CoreState } from "../../types";
import { AuthState } from "./types";

export const getters: GetterTree<AuthState, CoreState> = {
  isAuthenticated: (authState: AuthState) => !!authState.token,
  authStatus: (authState: AuthState) => authState.status,
  authToken: (authState: AuthState) => authState.token
};
