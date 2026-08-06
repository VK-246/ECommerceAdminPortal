import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: false,
  templateUrl: './dashboard-layout.component.html',
  styleUrls: ['./dashboard-layout.component.scss']
})
export class DashboardLayoutComponent implements OnInit {

  userEmail: string | null = '';
  userRole: string | null = '';

  // Navigation links in the sidebar — Epic 5 will add real routes here
  navLinks = [
    { label: 'Dashboard',   icon: 'dashboard',   route: '/dashboard' },
    { label: 'Categories',  icon: 'category',    route: '/dashboard/categories' },
    { label: 'Products',    icon: 'inventory_2', route: '/dashboard/products' }
  ];

  constructor(private authService: AuthService, private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.userEmail = this.authService.getUserEmail();
    this.userRole  = this.authService.getUserRole();
  }

  // Temporary method to verify the interceptor and cookies
  testApiCall(): void {
    this.http.get('http://localhost:5069/api/categories').subscribe({
      next: (res) => console.log('API Success (Check Network Tab):', res),
      error: (err) => console.error('API Error:', err)
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
