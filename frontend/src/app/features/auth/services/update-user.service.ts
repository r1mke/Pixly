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

  updateUser(data: { firstName: string; lastName: string; username: string; profileImg?: File; isImageDeleted?: boolean }): Observable<any> {
    const formData: FormData = new FormData();

    formData.append('firstName', data.firstName);
    formData.append('lastName', data.lastName);
    formData.append('username', data.username);

    if (data.profileImg) {
      formData.append('profileImg', data.profileImg, data.profileImg.name);
    }

    // Dodavanje statusa obrisane slike
    if (data.isImageDeleted) {
      formData.append('isImageDeleted', String(data.isImageDeleted)); // Pretvara boolean u string ("true" ili "false")
    }

    return this.http.put(this.apiUpdateUser, formData, { withCredentials: true }).pipe(
      catchError(this.authService.handleError)
    );
  }
}

