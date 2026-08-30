import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Attribute } from '../models/attribute.model';

@Injectable({
  providedIn: 'root'
})
export class AttributeService {
  private apiUrl = `${environment.apiUrl}/attributes`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Attribute[]>> {
    return this.http.get<ApiResponse<Attribute[]>>(this.apiUrl);
  }
}
