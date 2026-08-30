import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  /**
   * Called automatically by Angular's Router before navigating to a protected route.
   *
   * If the user has a token → return true (allow navigation).
   * If not → redirect to /login and block navigation (return false).
   *
   * This prevents users from accessing /dashboard just by typing the URL.
   */
  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}
