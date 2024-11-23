import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';

export interface AvailabilityResponse {
  available: boolean;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  private apiUrl = `${MYCONFIG.apiUrl}/auth/register`;

  constructor(private http: HttpClient) {}

  registerUser(userData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, userData, {
      withCredentials: true,
    });
  }
  
  checkEmail(email: string): Observable<AvailabilityResponse> {
    return this.http.get<AvailabilityResponse>(`${MYCONFIG.apiUrl}/auth/check-email?email=${email}`);
  }

  checkUsername(username: string): Observable<AvailabilityResponse> {
    return this.http.get<AvailabilityResponse>(`${MYCONFIG.apiUrl}/auth/check-username?username=${username}`);
  }
}
