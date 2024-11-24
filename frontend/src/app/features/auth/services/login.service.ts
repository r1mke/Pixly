import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
import { AuthService } from './auth.service';

export interface LoginResponse {
  jwtToken: string;
  message: string;
  user: any;
}

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private apiUrl = `${MYCONFIG.apiUrl}/auth/login`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  loginUser(loginData: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, loginData, {
      withCredentials: true,}).pipe(
      catchError(this.authService.handleError)
    );
  }
}
