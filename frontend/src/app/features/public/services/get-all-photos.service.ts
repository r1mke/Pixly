import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Observable } from 'rxjs';
import { HttpHeaders, HttpParams } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class GetAllPhotosService {

  private url = '/api/photos'

  private photos: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  public data$ = this.photos.asObservable();

  constructor(private http: HttpClient) { }

 // getPhotosRandom(): Observable<any> {
  //  return this.http.get<any>(this.url);
  //}
  //}
  
  getPhotosRandomAll(): void {
    this.http.get<any>(this.url).subscribe((data) => {
      this.photos.next(data);
    })
  }
}