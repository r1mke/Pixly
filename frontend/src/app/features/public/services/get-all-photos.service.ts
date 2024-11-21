import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Observable } from 'rxjs';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { MYCONFIG } from '../../../my-config';
@Injectable({
  providedIn: 'root'
})
export class GetAllPhotosService {

  private url = '/api/photos'


  constructor(private http: HttpClient) { }

  getAllPhotos() {
    return this.http.get<any[]>(`${MYCONFIG.apiUrl}/api/photos`);
  }
  
}