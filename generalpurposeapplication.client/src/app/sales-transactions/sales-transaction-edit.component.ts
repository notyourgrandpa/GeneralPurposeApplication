import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { Product } from '../products/product';
import { Observable, Subject, debounceTime, distinctUntilChanged, map, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../products/product.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { SalesTransactionService } from './sales-transaction.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SalesTransaction } from './sales-transaction';
import { Customer } from '../customers/customer';
import { CustomerService } from '../customers/customer.service';

@Component({
  selector: 'app-sales-transaction-edit',
  templateUrl: './sales-transaction-edit.component.html',
  styleUrl: './sales-transaction-edit.component.scss'
})
export class SalesTransactionEditComponent extends BaseFormComponent implements OnInit, OnDestroy {
  // the view title
  title?: string;

  // the product object to edit or create
  salesTransaction?: SalesTransaction;

  // the product object id, as fetched from the active route: It's NULL when we're adding a new product,
  // and not NULL when we're editing an existing one.
  id?: number;

  // the countries observable for the select (using async pipe)
  products?: Observable<Product[]>;

  // Activity Log (for debugging purposes)
  //activityLog: string = '';

  private destroySubject = new Subject();

  filteredCustomers!: Observable<Customer[]>;
  filteredProducts!: Observable<Product[]>;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private salesTransactionService: SalesTransactionService,
    private customerService: CustomerService,
    private productService: ProductService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      customer: new FormControl('', Validators.required),
      name: new FormControl(''),
      product: new FormControl(''),
      paymentMethod: new FormControl('Cash', Validators.required),
      //costPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      //sellingPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      //isActive: new FormControl('', Validators.required)
    }, null);

    // react to form changes
    this.form.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Form Model has been loaded.");
        }
        else {
          this.log("Form was updated by the user.");
        }
      });

    // react to changes in the form.name control
    this.form.get("name")!.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Name has been loaded with initial values.");
        }
        else {
          this.log("Name was updated by the user.");
        }
      });

    this.filteredCustomers = this.form.get('customer')!.valueChanges.pipe(
      debounceTime(300),            // wait until user stops typing
      distinctUntilChanged(),        // only if the text really changed
      switchMap(term => this.customerService.search(term || ''))
    );

    this.filteredProducts = this.form.get('product')!.valueChanges.pipe(
      debounceTime(300),            // wait until user stops typing
      distinctUntilChanged(),        // only if the text really changed
      switchMap(term => this.productService.search(term || ''))
    );

    this.loadData();
  }

  ngOnDestroy() {
    // emit a value with the takeUntil notifier
    this.destroySubject.next(true);
    // complete the subject
    this.destroySubject.complete();
  }

  log(str: string) {
    console.log("["
      +
      new Date().toLocaleString()
      + "] " + str);
  }

  loadData() {

    // load categories
    this.loadProducts();

    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE
      // fetch the product from the server
      this.salesTransactionService.get(this.id).subscribe({
        next: (result) => {
          this.salesTransaction = result;
          this.title = "Edit - " + this.salesTransaction.id;
          // update the form with the product value
          this.form.patchValue(this.salesTransaction);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Sales Transaction";

      // --- Option A: if you are sure Walk-in has Id = 1
      this.form.get('customer')!.setValue({ id: 1, name: 'Walk-in' });

      // --- Option B: fetch it dynamically
      // this.customerService.search('Walk-in')
      //   .pipe(map(list => list.find(c => c.name === 'Walk-in')))
      //   .subscribe(c => this.form.get('customer')!.setValue(c));
    }
  }

  loadProducts() {
    // fetch all the countries from the server
    this.products = this.salesTransactionService
      .getProducts(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }

  displayCustomer(customer: Customer): string {
    return customer ? customer.name : '';
  }

  displayProduct(product: Product): string {
    return product ? `${product.name} - ${product.sellingPrice}` + "" : '';
  }
}
