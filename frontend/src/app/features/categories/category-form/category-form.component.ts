import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: false,
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.scss']
})
export class CategoryFormComponent implements OnInit {
  categoryForm: FormGroup;
  isEditMode = false;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private dialogRef: MatDialogRef<CategoryFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data?: Category // Data passed into the dialog
  ) {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    if (this.data && this.data.id) {
      this.isEditMode = true;
      this.categoryForm.patchValue({
        name: this.data.name,
        description: this.data.description
      });
    }
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      return;
    }

    this.isSaving = true;
    const formData = this.categoryForm.value;

    if (this.isEditMode) {
      this.categoryService.update(this.data!.id, formData).subscribe({
        next: () => this.handleSuccess('Category updated'),
        error: (err) => this.handleError(err)
      });
    } else {
      this.categoryService.create(formData).subscribe({
        next: () => this.handleSuccess('Category created'),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleSuccess(message: string): void {
    this.isSaving = false;
    this.snackBar.open(message, 'Close', { duration: 3000 });
    this.dialogRef.close(true); // Close dialog and tell parent it was successful
  }

  private handleError(err: any): void {
    this.isSaving = false;
    const msg = err.error?.message || 'An error occurred';
    this.snackBar.open(msg, 'Close', { duration: 3000 });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
