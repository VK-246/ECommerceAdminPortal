import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginRequest, AuthResponse } from '../models/auth.model';
import { environment } from '../../../environments/environment';

import { ApiResponse } from '../models/api-response.model';

const TOKEN_KEY = 'ecommerce_token';
const EMAIL_KEY = 'ecommerce_email';
const ROLE_KEY  = 'ecommerce_role';

@Injectable({
  providedIn: 'root' // Singleton — only one instance exists for the whole app
})
export class AuthService {

  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient, private router: Router) {}

  /**
   * Sends login credentials to the backend.
   * Saves token, email, and role into localStorage and cookies for seamless session persistence.
   */
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          if (response.data.token) {
            localStorage.setItem(TOKEN_KEY, response.data.token);
          }
          localStorage.setItem(EMAIL_KEY, response.data.email);
          localStorage.setItem(ROLE_KEY,  response.data.role);
        }
      })
    );
  }

  /**
   * Tells the backend to clear the HttpOnly cookie, then clears UI data and redirects.
   */
  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      next: () => {
        this.clearSession();
      },
      error: () => {
        // Even if the backend call fails, clear local session and redirect
        this.clearSession();
      }
    });
  }

  private clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EMAIL_KEY);
    localStorage.removeItem(ROLE_KEY);
    this.router.navigate(['/login']);
  }

  /**
   * Returns the JWT token from localStorage.
   */
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  /**
   * Returns the logged-in user's email.
   */
  getUserEmail(): string | null {
    return localStorage.getItem(EMAIL_KEY);
  }

  /**
   * Returns the logged-in user's role (Admin or Editor).
   */
  getUserRole(): string | null {
    return localStorage.getItem(ROLE_KEY);
  }

  /**
   * Returns true if a user email exists in localStorage.
   */
  isLoggedIn(): boolean {
    return !!this.getUserEmail();
  }
}
