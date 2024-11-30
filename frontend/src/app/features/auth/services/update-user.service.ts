import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
import { AuthService } from './auth.service';


@Injectable({
  providedIn: 'root',
})
export class UpdateUserService {
  private apiUpdateUser = `${MYCONFIG.apiUrl}/user/update-user`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  updateUser(data: { firstName: string; lastName: string }): Observable<any> {
    return this.http.put(this.apiUpdateUser, data, { withCredentials: true }).pipe(
      catchError(this.authService.handleError)
    );
  }
  
}
