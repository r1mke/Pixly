import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../../../my-config';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class PhotoEndpointsService {

  constructor(private http: HttpClient) { }

  getAllPhotos() : Observable<any> {
    return this.http.get<any>(`${MYCONFIG.apiUrl}/api/admin/photos`);
  }

}
