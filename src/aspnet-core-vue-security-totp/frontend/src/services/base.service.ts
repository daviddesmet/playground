import { throwError } from "rxjs";

export abstract class BaseService {
  protected readonly api = "https://localhost:5000/api";

  protected handleError(error: any) {
    const serverError = "Service Unavailable";

    if (!error) {
      return throwError(serverError);
    }

    const applicationError = error.headers["Application-Error"];

    if (applicationError) {
      return throwError(applicationError);
    }

    let modelStateErrors: any = "";

    if (error.data) {
      for (const key in error.data) {
        if (error.data[key]) {
          modelStateErrors += error.data[key] + "\n";
        }
      }
    }

    modelStateErrors = modelStateErrors = "" ? null : modelStateErrors;
    return throwError(modelStateErrors || "Server Error");
  }
}
