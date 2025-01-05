import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
import { HttpParams } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUser = `${MYCONFIG.apiUrl}/user`;

  constructor(private http: HttpClient) {}

  getUserByUsername(username: string): Observable<any> {
    return this.http.get<any>(`${this.apiUser}/${username}`, { withCredentials: true }) ;
  }

  getUserByUsernameAdmin(username: string): Observable<any> {
    return this.http.get<any>(`${MYCONFIG.apiUrl}/admin/user/${username}`) ;
  }

  getUserLikedPhotos(username: string): Observable<any> {
    return this.http.get(`${this.apiUser}/${username}/liked-photos`, {withCredentials: true});
  }

  getAllUsers(request: any): Observable<any[]> {
    let params = new HttpParams();
    if (request.query) {
      params = params.append('query', request.query);
    }
    console.log(params); // Debugging; ukloniti u produkciji
    return this.http.get<any[]>(`${MYCONFIG.apiUrl}/api/users`, { params });
  }


}
