import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable, catchError, filter, of, switchMap, tap } from 'rxjs';
import { Product } from './product'; 
import { Category } from '../categories/category';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})

export class ProductService
  extends BaseService<Product> {
  constructor(
    http: HttpClient,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    super(http);
  }
  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<Product>> {
    var url = this.getUrl("api/products");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }
    return this.http.get<ApiResult<Product>>(url, { params });
  }
  get(id: number): Observable<Product> {
    var url = this.getUrl("api/products/" + id);
    return this.http.get<Product>(url);
  }
  put(item: Product): Observable<Product> {
    var url = this.getUrl("api/products/" + item.id);
    return this.http.put<Product>(url, item);
  }
  post(item: Product): Observable<Product> {
    var url = this.getUrl("api/products");
    return this.http.post<Product>(url, item);
  }

  delete(id: number): Observable<Product> {
    var url = this.getUrl("api/products/" + id);
    return this.http.delete<Product>(url);
  }

  getCategories(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<Category>> {
    var url = this.getUrl("api/Categories");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }
    return this.http.get<ApiResult<Category>>(url, { params });
  }
  isDupeProduct(item: Product): Observable<boolean> {
    var url = this.getUrl("api/Products/IsDupeProduct");
    return this.http.post<boolean>(url, item);
  }

  confirmAndDelete(id: number, redirectTo?: string, reloadCallback?: () => void): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: 'Are you sure you want to delete this product?'
      }
    }).afterClosed().pipe(
      filter(result => result === true),
      switchMap(() => this.delete(id)),
      tap(() => {
        this.snackBar.open('Product deleted successfully.', 'Close', { duration: 3000 });

        if (redirectTo) {
          this.router.navigate([redirectTo]);
        }

        if (reloadCallback) {
          reloadCallback();
        }
      }),
      catchError(err => {
        this.snackBar.open('Failed to delete the category.', 'Close', { duration: 3000 });
        console.error('Delete failed', err);
        return of(null);
      })
    ).subscribe();
  }

  search(term: string): Observable<Product[]> {
    return this.http.get<Product[]>(`/api/products/search`, {
      params: { term }
    });
  }
}
