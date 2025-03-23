import { Component, OnInit, ViewChild } from '@angular/core';
import { Data } from '@angular/router';
import { Product } from './product';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  public displayedColumns: string[] = [
    'id', 'name',
    'categoryName',
    'costPrice',
    'sellingPrice',
    'isActive',
    'dateAdded',
    'lastUpdated'
  ];
  public products!: MatTableDataSource<Product>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private http: HttpClient) {
  }

  ngOnInit() {
    this.http.get<Product[]>(environment.baseUrl + 'api/Products')
      .subscribe({
        next: (result) => {
          this.products = new MatTableDataSource<Product>(result);
          this.products.paginator = this.paginator;
        },
        error: (error) => console.error(error)
      })
  }
}
