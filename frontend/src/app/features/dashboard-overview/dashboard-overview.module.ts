import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';

import { DashboardOverviewRoutingModule } from './dashboard-overview-routing.module';
import { DashboardOverviewComponent } from './dashboard-overview.component';


@NgModule({
  declarations: [
    DashboardOverviewComponent
  ],
  imports: [
    CommonModule,
    DashboardOverviewRoutingModule,
    BaseChartDirective
  ]
})
export class DashboardOverviewModule { }
