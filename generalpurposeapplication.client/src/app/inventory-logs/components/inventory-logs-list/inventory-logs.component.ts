import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { InventoryLog } from '../../models/inventory-logs';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { InventoryLogService } from '../../services/inventory-logs.service';
import { ProductDialogService } from '../../../products/services/product-dialog.service'

@Component({
  selector: 'app-inventory-logs',
  templateUrl: './inventory-logs.component.html',
  styleUrl: './inventory-logs.component.scss'
})
export class InventoryLogsComponent implements OnInit {
  public displayedColumns: string[] = [
    'date',
    'productName',
    'quantity',
    'changeType',
    'remarks',
    'actions'
  ];

  public inventoryLogs!: MatTableDataSource<InventoryLog>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "date";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "date";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private inventoryLogService: InventoryLogService, private productDialogService: ProductDialogService) {
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

    this.inventoryLogService.getData(
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
          this.inventoryLogs = new MatTableDataSource<InventoryLog>(result.data);
        },
        error: (error) => console.error(error)
      });
  }

  onDelete(id: number): void {
    if (!id) return;
    this.inventoryLogService.confirmAndDelete(id, undefined, () => this.loadData());
  }

  openProductDialog(productId: number) {
    this.productDialogService.open(productId);
  }
}
