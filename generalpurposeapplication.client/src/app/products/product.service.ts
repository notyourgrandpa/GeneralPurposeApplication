import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';
import { Product } from './product'; 
import { Category } from '../categories/category';

@Injectable({
  providedIn: 'root',
})

export class ProductService
  extends BaseService<Product> {
  constructor(
    http: HttpClient) {
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
}
