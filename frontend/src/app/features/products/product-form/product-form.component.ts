import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Inject } from '@angular/core';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { MatChipInputEvent } from '@angular/material/chips';

import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { AiService } from '../../../core/services/ai.service';
import { AttributeService } from '../../../core/services/attribute.service';

import { Product, VariantRow, CreateProductRequest } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';
import { Attribute } from '../../../core/models/attribute.model';

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
  isGeneratingAi = false;
  categories: Category[] = [];
  attributes: Attribute[] = [];
  isLoadingCategories = true;
  isLoadingAttributes = true;

  // Variant builder state
  variantRows: VariantRow[] = [];
  variantsGenerated = false;

  // Chips config
  readonly separatorKeysCodes = [ENTER, COMMA] as const;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private aiService: AiService,
    private attributeService: AttributeService,
    private dialogRef: MatDialogRef<ProductFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data?: Product
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      categoryId: [null, Validators.required],
      description: [''],
      options: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadAttributes();

    if (this.data?.id) {
      this.isEditMode = true;
      this.productForm.patchValue({
        name: this.data.name,
        categoryId: this.data.categoryId,
        description: this.data.description
      });
      // Restore existing options from edit data
      if (this.data.options?.length) {
        this.data.options.forEach(opt => {
          this.optionsArray.push(this.fb.group({
            attributeId: [opt.attributeId, Validators.required],
            values: [opt.values.map(v => v.value)]
          }));
        });
        // Restore variant rows
        if (this.data.variants?.length) {
          this.variantRows = this.data.variants.map(v => ({
            label: v.optionValues.join(' / '),
            optionValueIndices: [],
            sku: v.sku,
            price: v.price,
            stockQuantity: v.stockQuantity
          }));
          this.variantsGenerated = true;
        }
      }
    }
  }

  get optionsArray(): FormArray {
    return this.productForm.get('options') as FormArray;
  }

  getOptionGroup(index: number): FormGroup {
    return this.optionsArray.at(index) as FormGroup;
  }

  getOptionValues(index: number): string[] {
    return this.getOptionGroup(index).get('values')?.value || [];
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

  loadAttributes(): void {
    this.attributeService.getAll().subscribe({
      next: (res) => {
        this.attributes = res.data;
        this.isLoadingAttributes = false;
      },
      error: () => {
        this.snackBar.open('Failed to load attributes', 'Close', { duration: 3000 });
        this.isLoadingAttributes = false;
      }
    });
  }

  addOption(): void {
    this.optionsArray.push(this.fb.group({
      attributeId: [null, Validators.required],
      values: [[]]
    }));
    this.variantsGenerated = false;
    this.variantRows = [];
  }

  removeOption(index: number): void {
    this.optionsArray.removeAt(index);
    this.variantsGenerated = false;
    this.variantRows = [];
  }

  addChip(event: MatChipInputEvent, optionIndex: number): void {
    const value = (event.value || '').trim();
    if (value) {
      const ctrl = this.getOptionGroup(optionIndex).get('values')!;
      const currentValues: string[] = ctrl.value || [];
      ctrl.setValue([...currentValues, value]);
    }
    event.chipInput!.clear();
    this.variantsGenerated = false;
    this.variantRows = [];
  }

  removeChip(optionIndex: number, chipValue: string): void {
    const ctrl = this.getOptionGroup(optionIndex).get('values')!;
    const currentValues: string[] = ctrl.value || [];
    ctrl.setValue(currentValues.filter(v => v !== chipValue));
    this.variantsGenerated = false;
    this.variantRows = [];
  }

  getAttributeName(attributeId: number): string {
    return this.attributes.find(a => a.id === attributeId)?.name || 'Option';
  }

  generateVariants(): void {
    const options = this.optionsArray.controls;

    // Validate all options have an attribute and at least one value
    const isValid = options.every(opt => {
      const g = opt as FormGroup;
      return g.get('attributeId')?.value && (g.get('values')?.value?.length > 0);
    });

    if (!isValid || options.length === 0) {
      this.snackBar.open('Each option needs an attribute and at least one value.', 'Close', { duration: 3000 });
      return;
    }

    // Compute cartesian product
    const valueSets: string[][] = options.map(opt => (opt as FormGroup).get('values')!.value);
    const cartesian = this.cartesianProduct(valueSets);

    this.variantRows = cartesian.map((combo, i) => ({
      label: combo.join(' / '),
      optionValueIndices: combo.map((val, optIdx) => valueSets[optIdx].indexOf(val)),
      sku: '',
      price: 0,
      stockQuantity: 0
    }));

    this.variantsGenerated = true;
    this.snackBar.open(`${this.variantRows.length} variant(s) generated!`, '✓', { duration: 2000 });
  }

  private cartesianProduct(sets: string[][]): string[][] {
    return sets.reduce<string[][]>(
      (acc, set) => acc.flatMap(combo => set.map(val => [...combo, val])),
      [[]]
    );
  }

  generateAiDescription(): void {
    const name = this.productForm.get('name')?.value;
    const categoryId = this.productForm.get('categoryId')?.value;

    if (!name) {
      this.snackBar.open('Please enter a product name first.', 'Close', { duration: 3000 });
      return;
    }

    const category = this.categories.find(c => c.id === categoryId)?.name;
    this.isGeneratingAi = true;

    this.aiService.generateDescription({ productName: name, categoryName: category }).subscribe({
      next: (res) => {
        this.productForm.patchValue({ description: res.data });
        this.isGeneratingAi = false;
        this.snackBar.open('AI Description generated!', '✓', { duration: 2000 });
      },
      error: (err) => {
        this.isGeneratingAi = false;
        const msg = err.error?.message || 'Failed to generate description';
        this.snackBar.open(msg, 'Close', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) return;
    if (!this.variantsGenerated || this.variantRows.length === 0) {
      this.snackBar.open('Please generate and configure at least one variant.', 'Close', { duration: 3000 });
      return;
    }

    const skuErrors = this.variantRows.filter(r => !r.sku.trim());
    if (skuErrors.length > 0) {
      this.snackBar.open('All variants need a SKU.', 'Close', { duration: 3000 });
      return;
    }

    const priceErrors = this.variantRows.filter(r => r.price <= 0);
    if (priceErrors.length > 0) {
      this.snackBar.open('All variants need a price greater than 0.', 'Close', { duration: 3000 });
      return;
    }

    const formValue = this.productForm.value;
    const payload: CreateProductRequest = {
      name: formValue.name,
      categoryId: formValue.categoryId,
      description: formValue.description,
      options: formValue.options.map((o: any) => ({
        attributeId: o.attributeId,
        values: o.values
      })),
      variants: this.variantRows.map(row => ({
        sku: row.sku.trim(),
        price: row.price,
        stockQuantity: row.stockQuantity,
        optionValueIndices: row.optionValueIndices
      }))
    };

    this.isSaving = true;

    if (this.isEditMode) {
      this.productService.update(this.data!.id, payload).subscribe({
        next: () => this.handleSuccess('Product updated successfully'),
        error: (err) => this.handleError(err)
      });
    } else {
      this.productService.create(payload).subscribe({
        next: () => this.handleSuccess('Product created successfully'),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleSuccess(message: string): void {
    this.isSaving = false;
    this.snackBar.open(message, '✓', { duration: 3000 });
    this.dialogRef.close(true);
  }

  private handleError(err: any): void {
    this.isSaving = false;
    const msg = err.error?.message || 'An error occurred';
    this.snackBar.open(msg, 'Close', { duration: 4000 });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
