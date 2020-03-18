import { ActionTree } from "vuex";
import { Credentials } from "@/models/credentials.interface";
import { Verification } from "@/models/verification.interface";
import { VerificationRecovery } from "@/models/verification.recovery.interface";
import { authService } from "@/services/auth.service";
import { CoreState } from "../../types";
import { AuthState } from "./types";
import {
  AUTH_REQUEST,
  AUTH_REFRESH_TOKEN,
  AUTH_VERIFICATION_REQUEST,
  AUTH_RECOVERY_REQUEST,
  AUTH_TWOFACTOR,
  AUTH_SUCCESS,
  AUTH_ERROR,
  AUTH_LOGOUT
} from "./actions.type";
import { Status, log } from "../timeline/utils";
import JwtService from "@/services/jwt.service";

export const actions: ActionTree<AuthState, CoreState> = {
  [AUTH_REQUEST]: (
    { commit, dispatch }: { commit: any; dispatch: any },
    credentials: Credentials
  ) => {
    return new Promise((resolve, reject) => {
      commit(AUTH_REQUEST);
      authService.login(credentials).subscribe(
        (result: any) => {
          if (result.status === 200 && result.token) {
            JwtService.saveToken(result.token);
            commit(AUTH_SUCCESS, result.token);

            log(
              dispatch,
              "User Login",
              "User successfully logged in",
              Status.Success
            );
            resolve(true);
          } else {
            // Status 206 - Authenticator
            JwtService.saveToken(result.token, "tfa_token");
            commit(AUTH_TWOFACTOR);
            resolve(false);
          }
        },
        (errors: any) => {
          commit(AUTH_ERROR, errors);
          JwtService.destroyToken();

          log(
            dispatch,
            "User Login",
            `Error while logging in: ${errors}`,
            Status.Error
          );
          reject(errors);
        }
      );
    });
  },
  [AUTH_REFRESH_TOKEN]: ({
    commit,
    dispatch
  }: {
    commit: any;
    dispatch: any;
  }) => {
    return new Promise((resolve, reject) => {
      authService.refresh().subscribe(
        (result: any) => {
          JwtService.saveToken(result);
          resolve();
        },
        (errors: any) => {
          JwtService.destroyToken();
          reject(errors);
        }
      );
    });
  },
  [AUTH_VERIFICATION_REQUEST]: (
    { commit, dispatch }: { commit: any; dispatch: any },
    verification: Verification
  ) => {
    return new Promise((resolve, reject) => {
      // commit(AUTH_REQUEST);
      authService.verify(verification).subscribe(
        (result: any) => {
          JwtService.saveToken(result);
          JwtService.destroyToken("tfa_token");
          commit(AUTH_SUCCESS, result);

          log(
            dispatch,
            "2FA Authenticator",
            "Authenticator's code verified",
            Status.Success
          );
          resolve();
        },
        (errors: any) => {
          commit(AUTH_ERROR, errors);
          JwtService.destroyToken();

          log(
            dispatch,
            "2FA Authenticator",
            `Error while verifying authenticator's code: ${errors}`,
            Status.Error
          );
          reject(errors);
        }
      );
    });
  },
  [AUTH_RECOVERY_REQUEST]: (
    { commit, dispatch }: { commit: any; dispatch: any },
    recovery: VerificationRecovery
  ) => {
    return new Promise((resolve, reject) => {
      // commit(AUTH_REQUEST);
      authService.recovery(recovery).subscribe(
        (result: any) => {
          JwtService.saveToken(result);
          JwtService.destroyToken("tfa_token");
          commit(AUTH_SUCCESS, result);

          log(
            dispatch,
            "2FA Recovery",
            "Recovery code verified",
            Status.Success
          );
          resolve();
        },
        (errors: any) => {
          commit(AUTH_ERROR, errors);
          JwtService.destroyToken();

          log(
            dispatch,
            "2FA Recovery",
            `Error while verifying recovery code: ${errors}`,
            Status.Error
          );
          reject(errors);
        }
      );
    });
  },
  [AUTH_LOGOUT]: ({ commit, dispatch }: { commit: any; dispatch: any }) => {
    return new Promise((resolve, reject) => {
      commit(AUTH_LOGOUT);
      JwtService.destroyToken();

      log(
        dispatch,
        "User Logout",
        "User successfully logged out",
        Status.Success
      );
      resolve();
    });
  }
};
