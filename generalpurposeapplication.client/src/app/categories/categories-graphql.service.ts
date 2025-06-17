import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable, map } from 'rxjs';
import { Category } from './category';
import { Apollo, gql } from 'apollo-angular';


@Injectable({
  providedIn: 'root',
})
export class CategoryGraphQlService
  extends BaseService<Category> {
  constructor(
    http: HttpClient, private apollo: Apollo) {
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
    return this.apollo
      .query({
        query: gql`
          query GetCategoryById($id: Int!){
            categories(where: {$id: {eq: $id } }){
              nodes {
                id
                name
              }
            }
          }
        `,
        variables: {
          id
        }
      })
      .pipe(map((result: any) =>
        result.data.categories.nodes[0]));
  }

  put(input: Category): Observable<Category> {
    //const dto = {
    //  id: input.id,
    //  name: input.name
    //};

    return this.apollo
      .mutate({
        mutation: gql`
          mutation UpdateCategory($category: CategoryInputDTO!){
            updateCategory(categoryDTO: $category){
              id
              name
            }
          }
        `,
        variables: {
          category: input
        }
      }).pipe(map((result: any) =>
        result.data.updateCategory));
  }

  post(item: Category): Observable<Category> {
    const dto = {
      name: item.name
    };
    return this.apollo
    .mutate({
      mutation: gql
        `mutation AddCategory($category: CategoryInputDTO!){
          addCategory(categoryDTO: $category){
            id
            name
          }
        }
      `,
      variables: {
        category: dto
      }
    }).pipe(map((result: any) =>
      result.data.addProduct));
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
