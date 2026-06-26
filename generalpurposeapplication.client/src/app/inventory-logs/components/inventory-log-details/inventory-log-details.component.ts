import { Component, Inject, OnInit } from '@angular/core';
import { InventoryLogService } from '../../services/inventory-logs.service';
import { InventoryLog } from '../../models/inventory-logs';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-inventory-log-details',
  templateUrl: './inventory-log-details.component.html',
  styleUrl: './inventory-log-details.component.css'
})
export class InventoryLogDetailsComponent implements OnInit {
  @Inject(MAT_DIALOG_DATA) public inventoryLogId: number = 0;
  inventoryLog? : InventoryLog

  constructor(
    private inventoryLogService: InventoryLogService, 
    private snackbar: MatSnackBar, 
    private dialog: MatDialog
  )
  {

  }

  ngOnInit(): void {
    this.getData(this.inventoryLogId);
  }

  getData(id: number): void{
    this.inventoryLogService.get(id).subscribe({
      next: (result) =>{
        this.inventoryLog = result;
      }
    });
  }
}
