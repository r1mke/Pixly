import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MYCONFIG } from '../../../my-config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiCurrentUser = `${MYCONFIG.apiUrl}/auth/current-user`;
  private apiLogout = `${MYCONFIG.apiUrl}/auth/logout`;

  private currentUserSubject = new BehaviorSubject<any | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  getCurrentUser(): Observable<any> {
    return this.http.get(this.apiCurrentUser, { withCredentials: true }).pipe(
      tap((res: any) => this.currentUserSubject.next(res.user)),
      catchError(this.handleError)
    );
  }

  logout(): Observable<any> {
    return this.http.post(this.apiLogout, null, { withCredentials: true }).pipe(
      tap(() => this.currentUserSubject.next(null)),
      catchError(this.handleError)
    );
  }

  public handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred.';

    if (error.status) {
      const errorResponse = error.error;

      if (errorResponse?.message) errorMessage = errorResponse.message;
    }
    return throwError(() => new Error(errorMessage));
  }
}
