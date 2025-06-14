import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable, map } from 'rxjs';
import { Product } from './product';
import { Category } from '../categories/category';

import { Apollo, gql } from 'apollo-angular';

@Injectable({
  providedIn: 'root',
})

export class ProductGraphQlService
  extends BaseService<Product> {
  constructor(
    http: HttpClient,
    private apollo: Apollo) {
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
    return this.apollo
      .query({
        query: gql`
          query GetProductsApiResult(
              $pageIndex: Int!,
              $pageSize: Int!,
              $sortColumn: String,
              $sortOrder: String,
              $filterColumn: String,
              $filterQuery: String) {
            productsApiResult(
              pageIndex: $pageIndex
              pageSize: $pageSize
              sortColumn: $sortColumn
              sortOrder: $sortOrder
              filterColumn: $filterColumn
              filterQuery: $filterQuery
            ) { 
               data { 
                 id
                 name
                 sellingPrice 
                 costPrice
                 isActive
                 categoryId
                 categoryName
                 dateAdded
                 lastUpdated
               },
                pageIndex
                pageSize
                totalCount
                totalPages
                sortColumn
                sortOrder
                filterColumn
                filterQuery
              }
          }
        `,
        variables: {
          pageIndex,
          pageSize,
          sortColumn,
          sortOrder,
          filterColumn,
          filterQuery
        }
      })
      .pipe(map((result: any) =>
        result.data.productsApiResult));
  }

  get(id: number): Observable<Product> {
    return this.apollo
      .query({
        query: gql`
          query GetProductById($id: Int!) {
            products(where: { id: { eq: $id } }) {
              nodes {
                id
                name
                sellingPrice
                costPrice
                isActive
                dateAdded
                lastUpdated
                categoryId
              }
            }
          }
        `,
        variables: {
          id
        }
      })
      .pipe(map((result: any) =>
        result.data.products.nodes[0]));
  }

  put(item: Product): Observable<Product> {
    var url = this.getUrl("api/products/" + item.id);
    return this.http.put<Product>(url, item);
  }

  post(item: Product): Observable<Product> {
    var url = this.getUrl("api/products");
    return this.http.post<Product>(url, item);
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
