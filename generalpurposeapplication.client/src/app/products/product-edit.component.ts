import { Component, OnInit, OnDestroy } from '@angular/core';
//import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

//import { environment } from '../../environments/environment';
import { Product } from './product';
import { Category } from './../categories/category';
import { BaseFormComponent } from './../base-form.component'
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';

import { ProductService } from './product.service';
import { ProductGraphQlService } from './product-graphql.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrl: './product-edit.component.scss'
})

export class ProductEditComponent extends BaseFormComponent implements OnInit, OnDestroy {
  // the view title
  title?: string;

  // the product object to edit or create
  product?: Product;

  // the product object id, as fetched from the active route: It's NULL when we're adding a new product,
  // and not NULL when we're editing an existing one.
  id?: number;

  // the countries observable for the select (using async pipe)
  categories?: Observable<Category[]>;

  // Activity Log (for debugging purposes)
  //activityLog: string = '';

  private destroySubject = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private productGraphQlService: ProductGraphQlService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      categoryId: new FormControl('', Validators.required),
      costPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      sellingPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      isActive: new FormControl('', Validators.required)
    }, null, this.isDupeProduct());

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
    this.loadCategories();

    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE
      // fetch the product from the server
      this.productGraphQlService.get(this.id).subscribe({
        next: (result) => {
          this.product = result;
          this.title = "Edit - " + this.product.name;
          // update the form with the product value
          this.form.patchValue(this.product);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Product";
    }
  }

  loadCategories() {
    // fetch all the countries from the server
    this.categories = this.productService
      .getCategories(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }
  
  onSubmit() {
    var product = (this.id) ? this.product : <Product>{};
    if (product) {
      product.name = this.form.controls['name'].value;
      product.categoryId = +this.form.controls['categoryId'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      product.isActive = this.form.controls['isActive'].value;

      if (this.id) {
        // EDIT mode
        this.productGraphQlService
          .put(product)
          .subscribe({
            next: (result) => {
              console.log("Product " + product!.id + " has been updated.");
              // go back to products view
              this.router.navigate(['/products']);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        this.productGraphQlService
          .post(product)
          .subscribe({
            next: (result) => {
              console.log("Product " + result.id + " has been created.");
              // go back to products view
              this.router.navigate(['/products']);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }

  onDelete(): void {
    if (!this.id) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: 'Are you sure you want to delete this product?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.productService.delete(this.id!).subscribe({
          next: () => {
            this.snackBar.open('Product deleted successfully.', 'Close', { duration: 3000 });
            this.router.navigate(['/products']);
          },
          error: (err) => {
            this.snackBar.open('Failed to delete the product.', 'Close', { duration: 3000 });
            console.error('Delete failed', err);
          }
        });
      }
    });
  }


  isDupeProduct(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var product = <Product>{};
      product.id = (this.id) ? this.id : 0;
      product.name = this.form.controls['name'].value;
      product.categoryId = +this.form.controls['categoryId'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      return this.productService.isDupeProduct(product).pipe(map(result => {
        return (result ? { isDupeProduct: true } : null);
      }));
    }
  }
}
