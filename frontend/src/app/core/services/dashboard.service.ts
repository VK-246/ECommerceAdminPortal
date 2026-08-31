import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface KpiDto {
  totalRevenue: number;
  totalOrders: number;
  lowStockAlerts: number;
}

export interface MonthlySalesDto {
  month: string;
  year: number;
  revenue: number;
}

export interface VariantAlertDto {
  sku: string;
  productName: string;
  stock: number;
}

export interface BestSellerDto {
  sku: string;
  productName: string;
  totalSold: number;
}

export interface InventoryAlertDto {
  lowStockVariants: VariantAlertDto[];
  bestSellers: BestSellerDto[];
}

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getKpis(): Observable<KpiDto> {
    return this.http.get<ApiResponse<KpiDto>>(`${this.apiUrl}/kpis`)
      .pipe(map(response => response.data));
  }

  getSalesChart(): Observable<MonthlySalesDto[]> {
    return this.http.get<ApiResponse<MonthlySalesDto[]>>(`${this.apiUrl}/sales-chart`)
      .pipe(map(response => response.data));
  }

  getInventoryAlerts(): Observable<InventoryAlertDto> {
    return this.http.get<ApiResponse<InventoryAlertDto>>(`${this.apiUrl}/inventory-alerts`)
      .pipe(map(response => response.data));
  }
}
