import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

// Angular Material Modules — imported here once and re-exported
// so any feature module that imports SharedModule gets all of these for free.
import { MatButtonModule }       from '@angular/material/button';
import { MatFormFieldModule }    from '@angular/material/form-field';
import { MatInputModule }        from '@angular/material/input';
import { MatCardModule }         from '@angular/material/card';
import { MatToolbarModule }      from '@angular/material/toolbar';
import { MatSidenavModule }      from '@angular/material/sidenav';
import { MatListModule }         from '@angular/material/list';
import { MatIconModule }         from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule }     from '@angular/material/snack-bar';
import { MatTableModule }        from '@angular/material/table';
import { MatPaginatorModule }    from '@angular/material/paginator';
import { MatDialogModule }       from '@angular/material/dialog';
import { MatSelectModule }       from '@angular/material/select';
import { MatMenuModule }         from '@angular/material/menu';
import { MatChipsModule }        from '@angular/material/chips';
import { MatTooltipModule }      from '@angular/material/tooltip';
import { MatDividerModule }      from '@angular/material/divider';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';

const MATERIAL_MODULES = [
  MatButtonModule,
  MatFormFieldModule,
  MatInputModule,
  MatCardModule,
  MatToolbarModule,
  MatSidenavModule,
  MatListModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatSnackBarModule,
  MatTableModule,
  MatPaginatorModule,
  MatDialogModule,
  MatSelectModule,
  MatMenuModule,
  MatChipsModule,
  MatTooltipModule,
  MatDividerModule,
];

/**
 * SharedModule is the "supply room" of the application.
 * Any feature module (AuthModule, ProductsModule, CategoriesModule) that needs
 * Angular Material components just imports SharedModule and gets everything.
 *
 * We import CommonModule and ReactiveFormsModule here too so they don't need to
 * be repeated in every feature module.
 */
@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  declarations: [
    ConfirmDialogComponent
  ]
})
export class SharedModule {}
