import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { MYCONFIG } from '../../../my-config';

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

  constructor(private http: HttpClient) {}

  // Funkcija za logovanje korisnika
  loginUser(loginData: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.apiUrl, loginData).pipe(
      catchError((error) => {
        let errorMessage = 'An error occurred'; // Default message
  
        // Proveri da li je greška došla sa servera
        if (error.error && error.error.message) {
          errorMessage = error.error.message; // Ako server vrati poruku, koristi tu poruku
        } else if (error.status === 400 || error.status === 401) {
          errorMessage = 'Invalid email or password'; // Specifična greška
        }
  
        return throwError(() => new Error(errorMessage)); // Vraćanje greške
      })
    );
  }
  

  // Čuvanje JWT tokena u localStorage
  saveToken(token: string): void {
    localStorage.setItem('jwtToken', token);
  }

  // Dobijanje JWT tokena iz localStorage
  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  // Brisanje JWT tokena iz localStorage
  removeToken(): void {
    localStorage.removeItem('jwtToken');
  }
}
