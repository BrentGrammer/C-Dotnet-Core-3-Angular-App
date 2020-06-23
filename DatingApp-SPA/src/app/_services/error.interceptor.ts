import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

// this is used to catch any responses in 400-500 range
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((resError) => {
        if (resError.status === 401) {
          return throwError(resError.statusText);
        }
        //now handle errors from the api - these are of type HttpErrorResponse
        // 500 type error messages returned are just a string
        if (resError instanceof HttpErrorResponse) {
          // the header for the error was setup on the backend in the extension method in helpers folder
          const applicationError = resError.headers.get('Application-Error');
          if (applicationError) {
            return throwError(applicationError);
          }
          const serverError = resError.error;
          // for form validation errors
          let modelStateErrors = '';
          // Angular sometimes produces some nested error objects and arrays, so these need to b checked for and handled
          if (serverError.errors && typeof serverError === 'object') {
            for (const key in serverError.errors) {
              if (serverError.errors[key]) {
                //building a string with all the errors
                modelStateErrors += serverError.errors[key] + '\n';
              }
            }
          }

          return throwError(modelStateErrors || serverError || 'Server Error');
        }
      })
    );
  }
}

// create a provider to register in ther app module using the errorinterceptor created above
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true,
};
