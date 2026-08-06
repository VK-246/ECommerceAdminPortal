import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './login/login.component';

/**
 * AuthModule is a self-contained feature module for the login screen.
 * It only knows about its own components and imports what it needs via SharedModule.
 * It is lazy-loaded — only fetched from the server when the user visits /login.
 */
@NgModule({
  declarations: [
    LoginComponent
  ],
  imports: [
    SharedModule,
    AuthRoutingModule
  ]
})
export class AuthModule {}
