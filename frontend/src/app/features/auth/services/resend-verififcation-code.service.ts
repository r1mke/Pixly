import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MYCONFIG } from '../../../my-config';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class ResendVerificationCodeService {
  private apiUrl = `${MYCONFIG.apiUrl}/auth/resend-verification-code`;

  constructor(private http: HttpClient, private authService:AuthService) {}

  resendVerificationCode(): Observable<any> {
    return this.http.post(this.apiUrl, {}, { withCredentials: true }).pipe(
      catchError(this.authService.handleError)
    );
  }

}
