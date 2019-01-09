import axios from "axios";
import { Observable, from } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { Credentials } from "@/models/credentials.interface";
import { Verification } from "@/models/verification.interface";
import { VerificationRecovery } from "@/models/verification.recovery.interface";
import { BaseService } from "./base.service";

class AuthService extends BaseService {
  private static instance: AuthService;

  private constructor() {
    super();
  }

  static get Instance() {
    // Do you need arguments? Make it a regular method instead.
    return this.instance || (this.instance = new this());
  }

  login(credentials: Credentials): Observable<any> {
    return (
      from(axios.post(`${this.api}/auth/login`, credentials))
        // TODO: use expires_in from response data to set the cookie expiration
        .pipe(
          map((res: any): { status: number; token: string } => ({ status: res.status, token: res.data.auth_token }))
        )
        .pipe(catchError((error: any) => this.handleError(error.response)))
    );
  }

  refresh(): Observable<any> {
    return (
      from(axios.get(`${this.api}/auth/refresh`))
        // TODO: use expires_in from response data to set the cookie expiration
        .pipe(map((res: any) => res.data.auth_token))
        .pipe(catchError((error: any) => this.handleError(error.response)))
    );
  }

  verify(verification: Verification): Observable<any> {
    return from(axios.post(`${this.api}/auth/2fa`, verification))
      .pipe(map((res: any) => res.data.auth_token))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }

  recovery(recovery: VerificationRecovery): Observable<any> {
    return from(axios.post(`${this.api}/auth/2fa/recovery`, recovery))
      .pipe(map((res: any) => res.data.auth_token))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }
}

// export a singleton instance in the global namespace
export const authService = AuthService.Instance;
