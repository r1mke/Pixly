import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MYCONFIG } from '../../../my-config';

@Injectable({
  providedIn: 'root',
})
export class VerifyEmailService {
  private apiUrl = `${MYCONFIG.apiUrl}/auth/verify-email`;

  constructor(private http: HttpClient) {}

  verifyEmail(code: string, email: string): Observable<any> {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const body = { verificationCode: code, email: email };

    return this.http.post(`${this.apiUrl}`, body, { headers })
    .pipe(
      catchError(this.handleError)
    );
  }
  
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred.';

    if (error.status === 400 || error.status === 422) {
      const errorResponse = error.error;

      if (errorResponse?.message) 
        errorMessage = errorResponse.message;
    }

    return throwError(() => new Error(errorMessage));
  }
}
