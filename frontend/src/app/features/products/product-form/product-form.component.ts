import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { Product } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  isEditMode = false;
  isSaving = false;
  categories: Category[] = [];
  isLoadingCategories = true;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private dialogRef: MatDialogRef<ProductFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data?: Product
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      sku: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadCategories();

    if (this.data && this.data.id) {
      this.isEditMode = true;
      this.productForm.patchValue({
        name: this.data.name,
        sku: this.data.sku,
        price: this.data.price,
        stockQuantity: this.data.stockQuantity,
        categoryId: this.data.categoryId
      });
    }
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        this.categories = res.data;
        this.isLoadingCategories = false;
      },
      error: () => {
        this.snackBar.open('Failed to load categories', 'Close', { duration: 3000 });
        this.isLoadingCategories = false;
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      return;
    }

    this.isSaving = true;
    const formData = this.productForm.value;

    if (this.isEditMode) {
      this.productService.update(this.data!.id, formData).subscribe({
        next: () => this.handleSuccess('Product updated'),
        error: (err) => this.handleError(err)
      });
    } else {
      this.productService.create(formData).subscribe({
        next: () => this.handleSuccess('Product created'),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleSuccess(message: string): void {
    this.isSaving = false;
    this.snackBar.open(message, 'Close', { duration: 3000 });
    this.dialogRef.close(true);
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
