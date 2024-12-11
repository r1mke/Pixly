import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MYCONFIG } from '../../../../my-config';
import { SearchRequest } from '../../model/SearchRequest';
import { SearchResult } from '../../model/SearchResult';

@Injectable({
  providedIn: 'root'
})
export class PhotosSearchService {
  constructor(private http : HttpClient) { }


  searchPhotos(request: SearchRequest): Observable<any> {
    
    let params = new HttpParams();

    if (request.Popularity) params = params.append('Popularity', request.Popularity);
    if (request.Title) params = params.append('Title', request.Title);
    if (request.Orientation) params = params.append('Orientation', request.Orientation);
    if (request.Size) params = params.append('Size', request.Size);
    if(request.Color) params = params.append('Color', request.Color);
    if(request.PageNumber) params = params.append('PageNumber', request.PageNumber);
    if(request.PageSize) params = params.append('PageSize', request.PageSize);

    console.log(params);

    return this.http.get<SearchResult>(`${MYCONFIG.apiUrl}/api/photos/search`, {params});
  }

}
