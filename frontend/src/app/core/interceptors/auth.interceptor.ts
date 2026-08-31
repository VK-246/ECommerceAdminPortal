import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  /**
   * Intercepts every outgoing HTTP request.
   *
   * 1. Appends withCredentials: true (for HttpOnly cookies).
   * 2. Attaches Authorization: Bearer <token> from localStorage (as a rock-solid backup).
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();

    const headersConfig: { [name: string]: string } = {};
    if (token) {
      headersConfig['Authorization'] = `Bearer ${token}`;
    }

    const authRequest = request.clone({
      withCredentials: true,
      setHeaders: headersConfig
    });

    return next.handle(authRequest);
  }
}
