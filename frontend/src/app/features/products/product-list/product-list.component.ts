import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { ProductFormComponent } from '../product-form/product-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'name', 'priceRange', 'totalStock', 'category', 'actions'];
  dataSource = new MatTableDataSource<Product>();
  isLoading = true;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private productService: ProductService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.productService.getAll().subscribe({
      next: (res) => {
        // res.data is a PagedResult containing the actual items
        this.dataSource.data = res.data.items;
        this.dataSource.paginator = this.paginator;
        this.isLoading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load products', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  openForm(product?: Product): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      width: '720px',
      maxWidth: '95vw',
      data: product
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProducts();
      }
    });
  }

  deleteProduct(product: Product): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Product',
        message: `Are you sure you want to permanently delete "${product.name}"? This action cannot be undone.`,
        confirmText: 'Delete',
        isDestructive: true
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.productService.delete(product.id).subscribe({
          next: () => {
            this.snackBar.open('Product deleted', 'Close', { duration: 3000 });
            this.loadProducts();
          },
          error: (err) => {
            const msg = err.error?.message || 'Failed to delete product';
            this.snackBar.open(msg, 'Close', { duration: 3000, panelClass: ['error-snackbar'] });
          }
        });
      }
    });
  }
}
