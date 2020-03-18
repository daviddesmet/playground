import axios from "axios";
import { Observable, from } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { BaseService } from "./base.service";
import { Authenticator } from "../models/authenticator.interface";
import { CredentialsChangePassword } from "../models/credentials.newpassword.interface";

class ProfileService extends BaseService {
  private static instance: ProfileService;

  private constructor() {
    super();
  }

  static get Instance() {
    // Do you need arguments? Make it a regular method instead.
    return this.instance || (this.instance = new this());
  }

  get(): Observable<any> {
    // const auth = {
    //   headers: { Authorization: `bearer ${localStorage.getItem("auth_token")}` }
    // };
    return from(axios.get(`${this.api}/profile/me` /*, auth*/))
      .pipe(map((res: any) => res.data))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  changePassword(credentials: CredentialsChangePassword): Observable<any> {
    return from(axios.post(`${this.api}/profile/password/change`, credentials))
      .pipe(map((res: any) => true))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  getAuthenticator(): Observable<any> {
    return from(axios.get(`${this.api}/profile/authenticator`))
      .pipe(map((res: any) => res.data))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  setAuthenticator(authenticator: Authenticator): Observable<any> {
    return from(
      axios.post(`${this.api}/profile/authenticator`, authenticator, {
        withCredentials: true
      })
    )
      .pipe(map((res: any) => true))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  getAuthenticatorCodes(): Observable<any> {
    return from(
      axios.get(`${this.api}/profile/authenticator/codes`, {
        withCredentials: true
      })
    )
      .pipe(map((res: any) => res.data))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  disableAuthenticator(): Observable<any> {
    return from(axios.post(`${this.api}/profile/authenticator/disable`))
      .pipe(map((res: any) => true))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  resetAuthenticator(): Observable<any> {
    return from(axios.post(`${this.api}/profile/authenticator/reset`))
      .pipe(map((res: any) => true))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }
}

// export a singleton instance in the global namespace
export const profileService = ProfileService.Instance;
