import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';
@Injectable({
  providedIn: 'root'
})
export class OpenaiService {
  private apiUrl = 'https://api.openai.com/v1/images/generations';
  private apiKey = MYCONFIG.openAi;


  constructor(private http: HttpClient) { }


  generateImage(prompt: string, quantity: number): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.apiKey}`
    });
    const body = {
      prompt: prompt,
      n: quantity,
      size: '1024x1024'
    };
    return this.http.post<any>(this.apiUrl, body, { headers });
  } 

}
