import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../my-config';
import { HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class GetAllPhotosService {
  pageNumber : number = 1;
  pageSize : number = 10;


  constructor(private http: HttpClient) { }
 
  getAllPhotos(pageNumber : number = this.pageNumber, pageSize : number = this.pageSize): Observable<any> {
      return this.http.get<any>(`${MYCONFIG.apiUrl}/api/photos/page/${pageNumber}/${pageSize}`, {withCredentials: true});
  }

  incrementPageNumber() {
    this.pageNumber++;
  }

  getCurrentPageNumber() {
    return this.pageNumber;
  }

  getPageSize() {
    return this.pageSize;
  }
  
  
}