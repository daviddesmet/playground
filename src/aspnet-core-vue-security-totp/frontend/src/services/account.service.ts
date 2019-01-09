import axios from "axios";
import { Observable, from } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { UserRegistration } from "@/models/user.registration.interface";
import { BaseService } from "./base.service";

class AccountService extends BaseService {
  private static instance: AccountService;

  private constructor() {
    super();
  }

  static get Instance() {
    // Do you need arguments? Make it a regular method instead.
    return this.instance || (this.instance = new this());
  }

  register(userRegistration: UserRegistration): Observable<any> {
    return from(axios.post(`${this.api}/accounts`, userRegistration))
      .pipe(map((res: any) => true))
      .pipe(catchError((error: any) => this.handleError(error.response)));
  }
}

// export a singleton instance in the global namespace
export const accountService = AccountService.Instance;
