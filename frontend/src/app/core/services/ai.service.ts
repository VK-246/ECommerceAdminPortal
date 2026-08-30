import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface GenerateDescriptionRequest {
  productName: string;
  categoryName?: string;
  additionalSpecs?: string;
}

export interface ChatRequest {
  prompt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AiService {
  private apiUrl = `${environment.apiUrl}/ai`;

  constructor(private http: HttpClient) {}

  generateDescription(request: GenerateDescriptionRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/generate-description`, request);
  }

  getMarketingAdvice(prompt: string): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/chat`, { prompt });
  }
}
