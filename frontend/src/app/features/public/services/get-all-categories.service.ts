import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MYCONFIG } from '../../../my-config';
import { HttpHeaders } from '@angular/common/http';
import { Category } from '../model/category';
@Injectable({
  providedIn: 'root'
})
export class GetAllCategoriesService {

  constructor(private http: HttpClient) { }

  getAllCategories() {
    return this.http.get<Category[]>(`${MYCONFIG.apiUrl}/api/categories/all`);
  }
}
