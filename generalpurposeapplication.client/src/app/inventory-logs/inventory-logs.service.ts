import { Injectable } from "@angular/core";
import { ApiResult, BaseService } from "../base.service";
import { InventoryLog } from "./inventory-logs";
import { Observable } from "rxjs";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Product } from "../products/product";

@Injectable({
  providedIn: 'root',
})

export class InventoryLogService extends BaseService<InventoryLog>{
  constructor(http: HttpClient) {
    super(http);
  }

  override getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null): Observable<ApiResult<InventoryLog>> {
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
    return this.http.get<ApiResult<InventoryLog>>(url, { params });
  }

  override get(id: number): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs/" + id);
    return this.http.get<InventoryLog>(url);
  }
  override put(item: InventoryLog): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs");
    return this.http.put<InventoryLog>(url, item);
  }
  override post(item: InventoryLog): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs");
    return this.http.post<InventoryLog>(url, item);
  }
  override delete(id: number): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs/" + id);
    return this.http.delete<InventoryLog>(url);
  }
}
