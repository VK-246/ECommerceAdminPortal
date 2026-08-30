import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { LayoutModule } from './layout/layout.module';

/**
 * AppModule is the root of the entire Angular application.
 * It imports the bare minimum needed to bootstrap the app:
 *  - BrowserModule:           Needed by every Angular browser app.
 *  - BrowserAnimationsModule: Required for Angular Material animations.
 *  - AppRoutingModule:        Sets up the routes.
 *  - CoreModule:              Registers our singleton services and interceptor.
 *  - LayoutModule:            Declares DashboardLayoutComponent (used directly in routing).
 *
 * Feature modules (AuthModule, ProductsModule etc.) are NOT imported here —
 * they are lazy-loaded by the router when needed.
 */
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    CoreModule,
    LayoutModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
