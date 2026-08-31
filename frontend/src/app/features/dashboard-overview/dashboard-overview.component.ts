import { Component, OnInit } from '@angular/core';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { DashboardService, KpiDto, MonthlySalesDto, InventoryAlertDto } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard-overview',
  standalone: false,
  templateUrl: './dashboard-overview.component.html',
  styleUrls: ['./dashboard-overview.component.scss']
})
export class DashboardOverviewComponent implements OnInit {
  kpis: KpiDto | null = null;
  inventoryAlerts: InventoryAlertDto | null = null;

  // Chart Data
  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      { data: [], label: 'Revenue ($)', backgroundColor: '#4F46E5', hoverBackgroundColor: '#6366F1' }
    ]
  };
  
  public barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };

  isLoading = true;

  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.isLoading = true;
    
    // Fetch KPIs
    this.dashboardService.getKpis().subscribe((data: KpiDto) => {
      this.kpis = data;
      this.checkLoading();
    });

    // Fetch Inventory Alerts
    this.dashboardService.getInventoryAlerts().subscribe((data: InventoryAlertDto) => {
      this.inventoryAlerts = data;
      this.checkLoading();
    });

    // Fetch Chart Data
    this.dashboardService.getSalesChart().subscribe((data: MonthlySalesDto[]) => {
      this.barChartData = {
        labels: data.map((d: MonthlySalesDto) => `${d.month} ${d.year}`),
        datasets: [
          { 
            data: data.map((d: MonthlySalesDto) => d.revenue), 
            label: 'Revenue ($)',
            backgroundColor: '#4F46E5', // Indigo-600
            hoverBackgroundColor: '#6366F1',
            borderRadius: 4
          }
        ]
      };
      this.checkLoading();
    });
  }

  private checkLoading(): void {
    if (this.kpis && this.inventoryAlerts && this.barChartData.labels?.length) {
      this.isLoading = false;
    }
  }
}
