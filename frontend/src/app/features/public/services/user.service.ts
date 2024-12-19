import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
import { Subject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUser = `${MYCONFIG.apiUrl}/user`;
  private updateAdminPhotos: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  updateAdminPhotos$ = this.updateAdminPhotos.asObservable();
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

  getAllUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${MYCONFIG.apiUrl}/api/users`);
  }

  updateAdminPhotosSubject(): void {
    const currentValue = this.updateAdminPhotos.getValue();
    this.updateAdminPhotos.next(!currentValue);
  }

}
