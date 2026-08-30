import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

import { ProductListComponent } from './product-list.component';
import { ProductService } from '../../../core/services/product.service';
import { SharedModule } from '../../../shared/shared.module';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let mockProductService: jasmine.SpyObj<ProductService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;
  let mockSnackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    mockProductService = jasmine.createSpyObj('ProductService', ['getAll', 'delete']);
    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);
    mockSnackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    mockProductService.getAll.and.returnValue(of({
      success: true,
      message: 'OK',
      data: {
        items: [
          {
            id: 1,
            name: 'Wireless Mouse',
            categoryId: 1,
            categoryName: 'Electronics',
            options: [],
            variants: [],
            priceRange: { min: 499, max: 599 },
            totalStock: 25
          }
        ],
        totalCount: 1,
        page: 1,
        pageSize: 10
      }
    }));

    await TestBed.configureTestingModule({
      declarations: [ProductListComponent],
      imports: [
        SharedModule,
        NoopAnimationsModule
      ],
      providers: [
        { provide: ProductService, useValue: mockProductService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: MatSnackBar, useValue: mockSnackBar }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load products with priceRange and totalStock columns', () => {
    expect(component).toBeTruthy();
    expect(component.dataSource.data.length).toBe(1);
    expect(component.displayedColumns).toContain('priceRange');
    expect(component.displayedColumns).toContain('totalStock');
    expect(component.isLoading).toBeFalse();
  });
});
