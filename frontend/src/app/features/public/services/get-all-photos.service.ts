import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../my-config';
import { HttpHeaders } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class GetAllPhotosService {

 
  constructor(private http: HttpClient) { }

  getAllPhotos() {
    return this.http.get<any[]>(`${MYCONFIG.apiUrl}/api/photos`);
  }

  getRandomPhoto() {
    return this.http.get<string>(`${MYCONFIG.apiUrl}/api/photo/random`, { responseType: 'text' as 'json' });
  }
  
}