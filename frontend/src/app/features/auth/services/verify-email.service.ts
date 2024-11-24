import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MYCONFIG } from '../../../my-config';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class VerifyEmailService {
  private apiUrl = `${MYCONFIG.apiUrl}/auth/verify-email`;

  constructor(private http: HttpClient, private authService:AuthService) {}

  verifyEmail(code: string, email: string): Observable<any> {
    const body = { verificationCode: code, email: email };

    return this.http.post(`${this.apiUrl}`, body, { withCredentials: true }).pipe(
      catchError(this.authService.handleError)
    );
  }
}
