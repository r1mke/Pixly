import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, of, throwError } from 'rxjs';
import { MYCONFIG } from '../../../my-config';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) {}
  private apiVerifyToken = `${MYCONFIG.apiUrl}/auth/verify-token`;
  private apiCurrentUser = `${MYCONFIG.apiUrl}/auth/current-user`;
  private apiLogout = `${MYCONFIG.apiUrl}/auth/logout`;

  /*
  verifyJwtToken(): Observable<any> {
    return this.http.get(this.apiUrl, { withCredentials: true }).pipe(
      catchError(err => {
        if (err.status === 401) {
          return of({ isValid: false });
        }
        return throwError(() => new Error(err));
      })
    );
  }
  */

  verifyJwtToken(): Observable<any> {
    return this.http.get(this.apiVerifyToken, { withCredentials: true }).pipe(
      catchError(this.handleError)
    );
  }

  getCurrentUser(): Observable<any> {
    return this.http.get(this.apiCurrentUser, { withCredentials: true }).pipe(
      catchError(this.handleError)
    );
  }

  logout(): Observable<any> {
    return this.http.post(this.apiLogout, null, { withCredentials: true }).pipe(
      catchError(this.handleError)
    );
  }

  handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred.';

    if (error.status) {
      const errorResponse = error.error;

      if (errorResponse?.message)
        errorMessage = errorResponse.message;
    }
    return throwError(() => new Error(errorMessage));
  }
}
