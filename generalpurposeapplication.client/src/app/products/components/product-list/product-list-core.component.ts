import { ProductQueryParams } from './../../models/product-query-params';
import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Subject, debounceTime, distinctUntilChanged, map, Observable } from 'rxjs';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Category } from '../../../categories/models/category';
import { CategoryQueryParams } from '../../../inventory-logs/models/category-query-params';
import { CategoryService } from '../../../categories/services/category.service';

@Component({
  selector: 'app-product-list-core',
  templateUrl: './product-list-core.component.html',
  styleUrl: './product-list-core.component.scss'
})
export class ProductListCoreComponent implements OnChanges {
  private readonly baseColumns: string[] = [
    'id',
    'name',
    'categoryName',
    'costPrice',
    'sellingPrice',
    'stock',
    'isActive',
    'dateAdded',
    'lastUpdated'
  ];
  public displayedColumns: string[] = [...this.baseColumns, 'actions'];
  public products: MatTableDataSource<Product> = new MatTableDataSource<Product>([]);
  public categories?: Observable<Category[]> ;
  @Input() categoryId?: number;
  @Input() compact = false;
  selectedCategoryId: number | null = null;
  selectedStatus: boolean | null = null;

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
    private productService: ProductService,
    private categoryService: CategoryService) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId']) {
      this.selectedCategoryId = this.categoryId ?? null;
    }

    if (changes['compact']) {
      this.updateDisplayedColumns();
    }
  }

  ngOnInit() {
    this.updateDisplayedColumns();
    this.loadData();
    this.loadCategories();
  }

  private updateDisplayedColumns(): void {
    this.displayedColumns = this.compact ? [...this.baseColumns] : [...this.baseColumns, 'actions'];
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
      : undefined;

    const categoryId = this.selectedCategoryId ?? undefined;
    const status = this.selectedStatus ?? undefined;
    const productQueryParams: ProductQueryParams = {
      pageIndex: event.pageIndex,
      pageSize: event.pageSize,
      filterQuery: filterQuery,
      filter: {
        categoryId: categoryId,
        isActive: status
      },
      sort: sortColumn,
      direction: sortOrder
    }

    this.productService.getProducts(productQueryParams).subscribe({
      next: (result) => {
        this.paginator.length = result.totalCount,
        this.paginator.pageIndex = result.pageIndex,
        this.paginator.pageSize = result.pageSize,
        this.products = new MatTableDataSource<Product>(result.data);
      },
      error: (error) => console.log(error)
    })

    //this.productService.getData(
    //  event.pageIndex,
    //  event.pageSize,
    //  sortColumn,
    //  sortOrder,
    //  filterColumn,
    //  filterQuery)
    //  .subscribe({
    //    next: (result) => {
    //      this.paginator.length = result.totalCount;
    //      this.paginator.pageIndex = result.pageIndex;
    //      this.paginator.pageSize = result.pageSize;
    //      this.products = new MatTableDataSource<Product>(result.data);
    //    },
    //    error: (error) => console.error(error)
    //  });
  }

  loadCategories(){
    const categoryQueryParams: CategoryQueryParams = {
      pageIndex: 0,
      pageSize: 999,
      sort: 'name',
      search: '',
      direction: 'asc'
    }
    this.categories = this.categoryService
      .getCategories(categoryQueryParams)
      .pipe(map(x => x.data));
  }

  onDelete(id: number): void {
    this.productService.confirmAndDelete(id, undefined, () => this.loadData());
  }

  onCategoryChanged(categoryId: number) {
    this.selectedCategoryId = categoryId;
    this.loadData();
  }

  onStatusChanged(status: boolean) {
    this.selectedStatus = status;
    this.loadData();
  }
}
