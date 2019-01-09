import { ActionTree } from "vuex";
import { Authenticator } from "@/models/authenticator.interface";
import { CredentialsChangePassword } from "@/models/credentials.newpassword.interface";
import { profileService } from "@/services/profile.service";
import { CoreState } from "../../types";
import { UserState } from "./types";
import {
  USER_REQUEST,
  USER_SUCCESS,
  USER_ERROR,
  USER_PASSWORD_CHANGE,
  AUTHENTICATOR_REQUEST,
  AUTHENTICATOR_ENABLE_REQUEST,
  AUTHENTICATOR_ENABLE_SUCCESS,
  AUTHENTICATOR_ENABLE_ERROR,
  AUTHENTICATOR_DISABLE_REQUEST,
  AUTHENTICATOR_DISABLE_SUCCESS,
  AUTHENTICATOR_DISABLE_ERROR,
  AUTHENTICATOR_RESET_REQUEST,
  AUTHENTICATOR_RESET_SUCCESS,
  AUTHENTICATOR_RESET_ERROR,
  AUTHENTICATOR_CODES_REQUEST
} from "./actions.type";
import { Status, log } from "../timeline/utils";
import { AUTH_LOGOUT } from "../auth/actions.type";

export const actions: ActionTree<UserState, CoreState> = {
  [USER_REQUEST]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    commit(USER_REQUEST);
    log(dispatch, "Profile Access", "Attempting request for user profile data", Status.Init);
    profileService.get().subscribe(
      (result: any) => {
        commit(USER_SUCCESS, result);
        log(dispatch, "Profile Access", "User profile data accessed", Status.Success);
      },
      (errors: any) => {
        commit(USER_ERROR);
        log(dispatch, "Profile Access", "Error while accessing the User profile data", Status.Error);
        dispatch(`auth/${AUTH_LOGOUT}`, null, { root: true });
      }
    );
  },
  [USER_PASSWORD_CHANGE]: (
    { commit, dispatch }: { commit: any; dispatch: any },
    credentials: CredentialsChangePassword
  ) => {
    return new Promise((resolve, reject) => {
      profileService.changePassword(credentials).subscribe(
        (result: any) => {
          log(dispatch, "Password Change", "User's password successfully changed", Status.Success);
          resolve(result);
        },
        (errors: any) => {
          reject(errors);
        }
      );
    });
  },
  [AUTHENTICATOR_REQUEST]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    return new Promise((resolve, reject) => {
      log(dispatch, "2FA Setup", "Attempting request for setup 2-Step verification", Status.Init);
      profileService.getAuthenticator().subscribe(
        (result: any) => {
          resolve(result);
        },
        (errors: any) => {
          reject(errors);
        }
      );
    });
  },
  [AUTHENTICATOR_ENABLE_REQUEST]: (
    {
      commit,
      dispatch
    }: {
      commit: any;
      dispatch: any;
    },
    authenticator: Authenticator
  ) => {
    return new Promise((resolve, reject) => {
      log(dispatch, "2FA Setup", "Attempting request for enabling 2-Step verification", Status.Init);
      profileService.setAuthenticator(authenticator).subscribe(
        (result: any) => {
          commit(AUTHENTICATOR_ENABLE_SUCCESS);
          log(dispatch, "2FA Enabled", "2-Step Verification have been successfully enabled", Status.Success);
          resolve(result);
        },
        (errors: any) => {
          commit(AUTHENTICATOR_ENABLE_ERROR);
          log(dispatch, "2FA Error", "2-Step Verification couldn't be enabled", Status.Error);
          reject(errors);
        }
      );
    });
  },
  [AUTHENTICATOR_CODES_REQUEST]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    return new Promise((resolve, reject) => {
      log(dispatch, "2FA Backup Codes", "Attempting request for 2-Step verification backup codes", Status.Init);
      profileService.getAuthenticatorCodes().subscribe(
        (result: any) => {
          log(dispatch, "2FA Backup Codes", "2-Step verification backup codes obtained", Status.Success);
          resolve(result);
        },
        (errors: any) => {
          log(dispatch, "2FA Backup Codes", "2-Step verification backup codes request error", Status.Error);
          reject(errors);
        }
      );
    });
  },
  [AUTHENTICATOR_DISABLE_REQUEST]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    return new Promise((resolve, reject) => {
      profileService.disableAuthenticator().subscribe(
        (result: any) => {
          commit(AUTHENTICATOR_DISABLE_SUCCESS);
          log(dispatch, "2FA Disabled", "2-Step Verification have been successfully disabled", Status.Success);
          resolve(result);
        },
        (errors: any) => {
          commit(AUTHENTICATOR_DISABLE_ERROR);
          reject(errors);
        }
      );
    });
  },
  [AUTHENTICATOR_RESET_REQUEST]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    return new Promise((resolve, reject) => {
      profileService.resetAuthenticator().subscribe(
        (result: any) => {
          commit(AUTHENTICATOR_RESET_SUCCESS);
          log(dispatch, "2FA Reset", "2-Step Verification have been successfully reset", Status.Success);
          resolve(result);
        },
        (errors: any) => {
          commit(AUTHENTICATOR_RESET_ERROR);
          log(dispatch, "2FA Reset", "2-Step Verification couldn't be reset", Status.Error);
          reject(errors);
        }
      );
    });
  }
};
