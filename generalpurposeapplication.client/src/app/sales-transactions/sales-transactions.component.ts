import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-sales-transactions',
  templateUrl: './sales-transactions.component.html',
  styleUrl: './sales-transactions.component.scss'
})
export class SalesTransactionsComponent implements OnInit {
  ngOnInit(): void {
      throw new Error('Method not implemented.');
  }
  public displayedColumns: string[] = [
    'id',
    'n'
  ]

}
