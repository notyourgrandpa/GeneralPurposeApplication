import { Component, OnInit, ViewChild } from '@angular/core';
//import { HttpClient, HttpParams } from '@angular/common/http';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
//import { environment } from '../../environments/environment';

import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';
import { CategoryGraphQlService } from '../../services/categories-graphql.service'
import { MatDialog } from '@angular/material/dialog';
import { ProductListDialogComponent } from '../../../products/components/product-list-dialog/product-list-dialog.component';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  public displayedColumns: string[] = [
    'name',
    'totalProducts',
    'action'
  ];
  public categories!: MatTableDataSource<Category>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "name";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(
    private categoryService: CategoryService,
    private categoryGraphQlService: CategoryGraphQlService,
    private dialog: MatDialog) {
  }

  ngOnInit() {
    this.loadData();
  }

  // debounce filter text changes
  onFilterTextChanged(filterText: string) {
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }

  loadData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;
    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;
    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;

    this.categoryGraphQlService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.categories = new MatTableDataSource<Category>(result.data);
        },
        error: (error) => console.error(error)
      });
  }

  onDelete(id: number): void {
    if (!id) return;
    this.categoryService.confirmAndDelete(id, undefined, () => this.loadData());
  }

  viewCategoryProducts(categoryId: number) {
    this.dialog.open(ProductListDialogComponent,
      {
        width: '800px',
        data: { categoryId }
      })
  }
}
