import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginRequest, AuthResponse } from '../models/auth.model';
import { environment } from '../../../environments/environment';

import { ApiResponse } from '../models/api-response.model';

// The keys we use to store UI state in the browser's localStorage
// The JWT token itself is NOT stored here anymore; it is safely inside an HttpOnly cookie.
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
   * On success, the backend sets an HttpOnly cookie containing the JWT.
   * We only save the email and role into localStorage for UI display purposes.
   */
  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success) {
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
    localStorage.removeItem(EMAIL_KEY);
    localStorage.removeItem(ROLE_KEY);
    this.router.navigate(['/login']);
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
   * This is a simple UI check. Actual security is enforced by the backend verifying the HttpOnly cookie.
   */
  isLoggedIn(): boolean {
    return !!this.getUserEmail();
  }
}
