import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { Product } from './product';
import { Category } from './../categories/category';

@Component({
  selector: 'app-product-edit',
  templateUrl: './product-edit.component.html',
  styleUrl: './product-edit.component.scss'
})

export class ProductEditComponent implements OnInit {
  // the view title
  title?: string;

  // the form model
  form!: FormGroup;

  // the product object to edit or create
  product?: Product;

  // the product object id, as fetched from the active route: It's NULL when we're adding a new product,
  // and not NULL when we're editing an existing one.
  id?: number;

  // the categories array for the select
  categories: Category[] = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient) {
  }
  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      categoryId: new FormControl('', Validators.required),
      costPrice: new FormControl('', Validators.required),
      sellingPrice: new FormControl('', Validators.required),
      isActive: new FormControl('', Validators.required)
    }, null, this.isDupeProduct());
    this.loadData();
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
      var url = environment.baseUrl + 'api/Products/' + this.id;
      this.http.get<Product>(url).subscribe({
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
    // fetch all the categories from the server
    var url = environment.baseUrl + 'api/Categories';
    var params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name");

    this.http.get<any>(url, { params }).subscribe({
      next: (result) => {
        this.categories = result.data;
        console.log("Categories: ");
        console.log(result.data);
      },
      error: (error) => console.error(error)
    });
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
        var url = environment.baseUrl + 'api/Products/' + product.id;
        this.http
          .put<Product>(url, product)
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
        var url = environment.baseUrl + 'api/Products';
        this.http
          .post<Product>(url, product)
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

  isDupeProduct(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var product = <Product>{};
      product.id = (this.id) ? this.id : 0;
      product.name = this.form.controls['name'].value;
      product.categoryId = +this.form.controls['categoryId'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      var url = environment.baseUrl + 'api/Products/IsDupeProduct';
      return this.http.post<boolean>(url, product).pipe(map(result => {
        return (result ? { isDupeProduct: true } : null);
      }));
    }
  }
}
