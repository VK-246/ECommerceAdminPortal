import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

/**
 * CoreModule is imported ONCE by AppModule only.
 * It provides singleton services (AuthService, AuthGuard) and registers the interceptor.
 *
 * Why a separate CoreModule?
 * It enforces a clear rule: things in here are "global" and should never be duplicated.
 * If a developer accidentally imports CoreModule a second time, it can cause bugs
 * (e.g., two instances of AuthService — one with a token, one without).
 */
@NgModule({
  imports: [
    HttpClientModule
  ],
  providers: [
    // Register the AuthInterceptor into Angular's HTTP pipeline.
    // Every HTTP request in the entire app will now pass through it automatically.
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true // 'multi: true' means "add to the list" instead of "replace the list"
    }
  ]
})
export class CoreModule {}
