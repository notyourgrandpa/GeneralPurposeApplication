import { Observable } from "rxjs";
import { ApiResult, BaseService } from "../../shared/services/base.service";
import { ExpensesModel } from "./expenses.model";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root',
})

export class ExpensesService extends BaseService<ExpensesModel> {
  override get(id: number): Observable<ExpensesModel> {
      throw new Error("Method not implemented.");
  }
  override put(item: ExpensesModel): Observable<ExpensesModel> {
      throw new Error("Method not implemented.");
  }
  override post(item: ExpensesModel): Observable<ExpensesModel> {
      throw new Error("Method not implemented.");
  }
  override delete(id: number): Observable<ExpensesModel> {
      throw new Error("Method not implemented.");
  }
  constructor(http: HttpClient)
  {
    super(http);
  }

  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null): Observable<ApiResult<ExpensesModel>> {
    var url = this.getUrl("api/expenses");
    var params = new HttpParams()
      .set("pageIndex", pageIndex)
      .set("pageSize", pageSize)
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder)
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery)
    }
    return this.http.get<ApiResult<ExpensesModel>>(url, {params})
  }
}
