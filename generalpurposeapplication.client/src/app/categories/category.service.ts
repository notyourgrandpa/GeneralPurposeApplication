import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';
import { Category } from './category';
@Injectable({
  providedIn: 'root',
})
export class CategoryService
  extends BaseService<Category> {
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
  isDupeField(categoryId: number, fieldName: string, fieldValue: string):
    Observable<boolean> {
    var params = new HttpParams()
      .set("categoryId", categoryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);
    var url = this.getUrl("api/Categories/IsDupeField");
    return this.http.post<boolean>(url, null, { params });
  }
}
