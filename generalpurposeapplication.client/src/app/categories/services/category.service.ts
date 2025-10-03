import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../../shared/services/base.service';
import { Observable, filter, switchMap, tap, catchError, of } from 'rxjs';
import { Category } from '../models/category';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class CategoryService
  extends BaseService<Category> {
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

  get(id: number): Observable<Category> {
    var url = this.getUrl("api/Categories/" + id);
    return this.http.get<Category>(url);
  }

  put(item: Category): Observable<Category> {
    var url = this.getUrl("api/Categories/" + item.id);
    return this.http.put<Category>(url, item);
  }

  post(item: Category): Observable<Category> {
    var url = this.getUrl("api/Categories");
    return this.http.post<Category>(url, item);
  }

  delete(id: number): Observable<Category> {
    var url = this.getUrl("api/Categories/" + id);
    return this.http.delete<Category>(url);
  }

  isDupeField(categoryId: number, fieldName: string, fieldValue: string):
    Observable<boolean> {
    var params = new HttpParams()
      .set("categoryId", categoryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);
    var url = this.getUrl("api/Categories/IsDupeField");
    return this.http.post<boolean>(url, null, { params });
  }

  confirmAndDelete(id: number, redirectTo?: string, reloadCallback?: () => void): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Category',
        message: 'Are you sure you want to delete this category?'
      }
    }).afterClosed().pipe(
      filter(result => result === true),
      switchMap(() => this.delete(id)),
      tap(() => {
        this.snackBar.open('Category deleted successfully.', 'Close', { duration: 3000 });

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

}
