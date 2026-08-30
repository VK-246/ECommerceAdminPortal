import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ProductFormComponent } from './product-form.component';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { AiService } from '../../../core/services/ai.service';
import { AttributeService } from '../../../core/services/attribute.service';
import { SharedModule } from '../../../shared/shared.module';

describe('ProductFormComponent', () => {
  let component: ProductFormComponent;
  let fixture: ComponentFixture<ProductFormComponent>;
  let mockProductService: jasmine.SpyObj<ProductService>;
  let mockCategoryService: jasmine.SpyObj<CategoryService>;
  let mockAiService: jasmine.SpyObj<AiService>;
  let mockAttributeService: jasmine.SpyObj<AttributeService>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<ProductFormComponent>>;
  let mockSnackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    mockProductService = jasmine.createSpyObj('ProductService', ['create', 'update']);
    mockCategoryService = jasmine.createSpyObj('CategoryService', ['getAll']);
    mockAiService = jasmine.createSpyObj('AiService', ['generateDescription']);
    mockAttributeService = jasmine.createSpyObj('AttributeService', ['getAll']);
    mockDialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);
    mockSnackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    mockCategoryService.getAll.and.returnValue(of({
      success: true,
      message: 'OK',
      data: [{ id: 1, name: 'Clothing', description: 'Apparel' }]
    }));

    mockAttributeService.getAll.and.returnValue(of({
      success: true,
      message: 'OK',
      data: [
        { id: 1, name: 'Color' },
        { id: 2, name: 'Size' }
      ]
    }));

    await TestBed.configureTestingModule({
      declarations: [ProductFormComponent],
      imports: [
        ReactiveFormsModule,
        SharedModule,
        NoopAnimationsModule
      ],
      providers: [
        FormBuilder,
        { provide: ProductService, useValue: mockProductService },
        { provide: CategoryService, useValue: mockCategoryService },
        { provide: AiService, useValue: mockAiService },
        { provide: AttributeService, useValue: mockAttributeService },
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MatSnackBar, useValue: mockSnackBar },
        { provide: MAT_DIALOG_DATA, useValue: null }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component and load categories and attributes on init', () => {
    expect(component).toBeTruthy();
    expect(component.categories.length).toBe(1);
    expect(component.attributes.length).toBe(2);
    expect(component.productForm.valid).toBeFalse();
  });

  it('should add and remove option rows dynamically', () => {
    expect(component.optionsArray.length).toBe(0);
    component.addOption();
    expect(component.optionsArray.length).toBe(1);
    component.removeOption(0);
    expect(component.optionsArray.length).toBe(0);
  });

  it('should generate variants via cartesian product calculation', () => {
    // Add Color: Red, Blue
    component.addOption();
    const opt1 = component.getOptionGroup(0);
    opt1.patchValue({ attributeId: 1, values: ['Red', 'Blue'] });

    // Add Size: S, M
    component.addOption();
    const opt2 = component.getOptionGroup(1);
    opt2.patchValue({ attributeId: 2, values: ['S', 'M'] });

    component.generateVariants();

    expect(component.variantsGenerated).toBeTrue();
    expect(component.variantRows.length).toBe(4);
    expect(component.variantRows[0].label).toBe('Red / S');
    expect(component.variantRows[1].label).toBe('Red / M');
    expect(component.variantRows[2].label).toBe('Blue / S');
    expect(component.variantRows[3].label).toBe('Blue / M');
  });

  it('should prevent submission when variants are not configured', () => {
    component.productForm.patchValue({
      name: 'Test Hoodie',
      categoryId: 1
    });

    component.onSubmit();
    expect(mockProductService.create).not.toHaveBeenCalled();
    expect(mockSnackBar.open).toHaveBeenCalledWith(
      jasmine.stringMatching(/variant/i),
      'Close',
      jasmine.any(Object)
    );
  });

  it('should submit product with variant payload when all fields are valid', () => {
    component.productForm.patchValue({
      name: 'Test Hoodie',
      categoryId: 1,
      description: 'Cozy hoodie'
    });

    component.addOption();
    component.getOptionGroup(0).patchValue({ attributeId: 1, values: ['Black'] });
    component.generateVariants();

    component.variantRows[0].sku = 'HD-BLK';
    component.variantRows[0].price = 1200;
    component.variantRows[0].stockQuantity = 15;

    mockProductService.create.and.returnValue(of({
      success: true,
      message: 'Created',
      data: {} as any
    }));

    component.onSubmit();

    expect(mockProductService.create).toHaveBeenCalledWith(jasmine.objectContaining({
      name: 'Test Hoodie',
      categoryId: 1,
      variants: [jasmine.objectContaining({
        sku: 'HD-BLK',
        price: 1200,
        stockQuantity: 15
      })]
    }));
    expect(mockDialogRef.close).toHaveBeenCalledWith(true);
  });
});
