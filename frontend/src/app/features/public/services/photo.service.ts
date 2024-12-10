import { Injectable } from '@angular/core';
import { MYCONFIG } from '../../../my-config';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  private apiUrl = `${MYCONFIG.apiUrl}/api/photos`;

  constructor(private http: HttpClient) {}

  getPhotoById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`, { withCredentials: true });
  }

  likePhoto(photoId: number, userId: number): Observable<any> {
    return this.http.post<any>(
      `${MYCONFIG.apiUrl}/api/photos/${photoId}/like?userId=${userId}`, 
      {}
    );
  }
  
  unlikePhoto(photoId: number, userId: number): Observable<any> {
    return this.http.delete<any>(
      `${MYCONFIG.apiUrl}/api/photos/${photoId}/like?userId=${userId}`
    );
  }
}
