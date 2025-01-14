import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
import { AuthService } from './auth.service';

export interface LoginResponse {
  statusCode: number;
  jwtToken: string;
  message: string;
  user: any;
}

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private apiUrl = `${MYCONFIG.apiUrl}`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  loginUser(loginData: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, loginData, {
      withCredentials: true,}).pipe(
      catchError(this.authService.handleError)
    );
  }

  verify2FA(code: string): Observable<any> {
    const verificationData = { code };
    return this.http.post(`${this.apiUrl}/auth/verify-2fa`, verificationData, {withCredentials: true})
    .pipe(catchError(this.authService.handleError));
  }

  resend2FA(): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/resend-2fa`, {}, { withCredentials: true }).pipe(
      catchError(this.authService.handleError)
    );
  }
}
