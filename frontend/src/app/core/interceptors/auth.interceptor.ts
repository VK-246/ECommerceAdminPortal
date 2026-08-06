import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  /**
   * Intercepts every outgoing HTTP request.
   *
   * Appends `withCredentials: true` to tell the browser to automatically include
   * the HttpOnly 'ecommerce_token' cookie with every API call.
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Clone the request and enable credentials (cookies)
    const authRequest = request.clone({
      withCredentials: true
    });
    return next.handle(authRequest);
  }
}
