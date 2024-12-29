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

  updatePhoto(id:number, data: {title : string, description: string, location: string}): Observable<any> {
    const formData: FormData = new FormData();

    formData.append('title', data.title);
    formData.append('description', data.description);
    formData.append('location', data.location);

    return this.http.put<any>(`${MYCONFIG.apiUrl}/api/photos/${id}`, formData, {withCredentials: true});
  }

  approvedPhoto(id:number, approved: boolean): Observable<any> {
    const payload = {
      PhotoId: id,
      Approved: approved,
    };
  
    return this.http.put<any>(`${MYCONFIG.apiUrl}/api/photoApproved`, payload, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  deletePhotoById(id: number): Observable<any> {
    return this.http.delete<any>(`${MYCONFIG.apiUrl}/api/photos/${id}`);
  }
}
