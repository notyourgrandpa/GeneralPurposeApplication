import { Component } from '@angular/core';
import { AdministrationService } from '../../services/administration.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-data-maintenance',
  templateUrl: './data-maintenance.component.html',
  styleUrl: './data-maintenance.component.css'
})
export class DataMaintenanceComponent {

  constructor(
    private adminService: AdministrationService,
    private snackBar: MatSnackBar
  ){

  }

  onImport(){
    console.log("wtf");
    this.adminService
    .importData()
    .subscribe({
      next: (result) => {
        console.log(result);
        this.snackBar.open(`Imported ${result.categories} categories and ${result.products} products.`, "Ok", { duration: 3000 });
      },
      error: (error) =>{
        this.snackBar.open(error, "Ok", { duration: 5000 });
      }
    })
  }
}
