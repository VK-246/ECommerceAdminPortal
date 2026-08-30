import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  isDestructive?: boolean;
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: false,
  template: `
    <div class="p-6 pb-2 relative overflow-hidden rounded-3xl">
      <!-- Glow effect -->
      <div class="absolute top-0 right-0 w-48 h-48 bg-red-600/10 rounded-full blur-[60px] pointer-events-none" *ngIf="data.isDestructive"></div>
      
      <div class="flex items-center gap-4 mb-4 relative z-10">
        <div [ngClass]="data.isDestructive ? 'bg-red-500/20 border-red-500/30' : 'bg-indigo-500/20 border-indigo-500/30'" 
             class="flex items-center justify-center w-12 h-12 rounded-full border shadow-lg">
          <mat-icon [ngClass]="data.isDestructive ? 'text-red-400' : 'text-indigo-400'">
            {{ data.isDestructive ? 'warning' : 'help_outline' }}
          </mat-icon>
        </div>
        <h2 class="text-xl font-bold text-white tracking-tight m-0" style="border: none; padding: 0; margin: 0;">{{ data.title }}</h2>
      </div>
      
      <mat-dialog-content class="relative z-10 text-slate-300 mb-6" style="padding-left: 0; padding-right: 0;">
        <p class="text-[15px] leading-relaxed">{{ data.message }}</p>
      </mat-dialog-content>
      
      <mat-dialog-actions align="end" class="relative z-10 gap-3" style="padding-bottom: 0;">
        <button mat-button (click)="onCancel()" class="text-slate-400 hover:text-white px-4 py-2 rounded-full transition-colors">
          {{ data.cancelText || 'Cancel' }}
        </button>
        <button mat-button (click)="onConfirm()" 
                [ngClass]="data.isDestructive ? 'bg-red-600 hover:bg-red-500 shadow-[0_0_15px_rgba(220,38,38,0.3)]' : 'bg-indigo-600 hover:bg-indigo-500 shadow-[0_0_15px_rgba(99,102,241,0.3)]'"
                class="text-white px-6 py-2 rounded-full font-medium transition-all hover:scale-[1.02]">
          {{ data.confirmText || 'Confirm' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    ::ng-deep .mat-mdc-dialog-container .mdc-dialog__surface {
      padding: 0 !important;
    }
  `]
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {
    this.data.isDestructive = this.data.isDestructive ?? true;
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
