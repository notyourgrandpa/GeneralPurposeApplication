import { Component, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { InventoryLog } from './inventory-logs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { InventoryLogService } from './inventory-logs.service';

@Component({
  selector: 'app-inventory-logs',
  templateUrl: './inventory-logs.component.html',
  styleUrl: './inventory-logs.component.scss'
})
export class InventoryLogsComponent {
  public displayedColumns: string[] = [
    'date',
    'product',
    'change',
    'remarks'
  ];

  public inventoryLogs!: MatTableDataSource<InventoryLog>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "date";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "productName";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private inventoryLogService: InventoryLogService) {
  }
}
