import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../my-config';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PostPhoto } from '../model/PostPhoto';
@Injectable({
  providedIn: 'root'
})
export class PhotoPostService {

  constructor(private http: HttpClient) { }

  postPhoto(data: FormData) : Observable<any> {
    const url = `${MYCONFIG.apiUrl}/api/photos`;
    
    return this.http.post<any>(url, data);
  }

 
  
  

}
