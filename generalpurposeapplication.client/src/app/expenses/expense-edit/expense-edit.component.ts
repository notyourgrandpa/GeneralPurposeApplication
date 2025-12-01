import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ExpensesService } from '../services/expenses.service';
import { ExpensesModel } from '../models/expenses.model';
import { BaseFormComponent } from '../../shared/components/base-form.component';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-expense-edit',
  templateUrl: './expense-edit.component.html',
  styleUrl: './expense-edit.component.scss'
})
export class ExpenseEditComponent extends BaseFormComponent implements OnInit {
  title?: string;
  id?: number;
  expense?: ExpensesModel;

  constructor(private activatedRoute: ActivatedRoute,
    private expensesService: ExpensesService)
  {
    super()
  }

  ngOnInit() {
    this.form = new FormGroup({
      category: new FormControl('', Validators.required),
      description: new FormControl('', Validators.required),
      amount: new FormControl('', [Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      date: new FormControl('', Validators.required)
    }, null)
    this.loadData();
  }

  ngOnSubmit() {

  }

  onDelete(){

  }

  loadData() {
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      this.expensesService.get(this.id).subscribe({
        next: (result) => {
          this.expense = result;
        }
      });
    }
    else {
      this.title = "Create a new Expense";
    }
  }
}
