import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { DashboardLayoutComponent } from './dashboard-layout/dashboard-layout.component';

/**
 * LayoutModule owns the persistent shell of the application.
 * It is imported directly by AppModule (not lazy-loaded) because the shell
 * needs to be available immediately for routing to work.
 */
@NgModule({
  declarations: [
    DashboardLayoutComponent
  ],
  imports: [
    SharedModule,
    RouterModule
  ],
  exports: [
    DashboardLayoutComponent
  ]
})
export class LayoutModule {}
