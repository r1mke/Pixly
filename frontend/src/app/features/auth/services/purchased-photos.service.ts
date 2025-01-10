import { Injectable } from "@angular/core";
import { MYCONFIG } from '../../../my-config';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class PurchasedPhotosService{
  private api = `${MYCONFIG.apiUrl}/photos/get-purchased-photos`;

  constructor(private http: HttpClient) {}

  getPurchasedPhotos(): Observable<any> {
    return this.http.get(this.api, { withCredentials: true });
  }
}