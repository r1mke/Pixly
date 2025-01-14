import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../../my-config';
import { Observable } from 'rxjs';
import { PhotoGetAllRequest } from '../../model/PhotoGetAllRequest';
import { PhotoGetAllResult } from '../../model/PhotoGetAllResult';
@Injectable({
  providedIn: 'root'
})
export class GetAllPhotosService {

  constructor(private http: HttpClient) { }
 
  getAllPhotos(request: PhotoGetAllRequest): Observable<any> {
    const params = {
      pageNumber: request.PageNumber,
      pageSize: request.PageSize,
    };
      return this.http.get<PhotoGetAllResult>(`${MYCONFIG.apiUrl}/api/photos`, {params, withCredentials: true});
  }

  
}