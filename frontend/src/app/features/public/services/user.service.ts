import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUser = `${MYCONFIG.apiUrl}/user`;

  constructor(private http: HttpClient) {}

  getUserByUsername(username: string): Observable<any> {
    return this.http.get(`${this.apiUser}/${username}`, { withCredentials: true }) ;
  }

  getUserLikedPhotos(username: string): Observable<any> {
    return this.http.get(`${this.apiUser}/${username}/liked-photos`, {withCredentials: true});
  }
}
