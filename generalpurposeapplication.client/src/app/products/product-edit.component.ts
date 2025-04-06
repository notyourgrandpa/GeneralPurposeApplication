import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';

import { environment } from '../../environments/environment';
import { Product } from './product';

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
  // the product object to edit
  product?: Product;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient) {
  }
  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      costPrice: new FormControl(''),
      sellingPrice: new FormControl(''),
      isActive: new FormControl(''),
      dateAdded: new FormControl(''),
      lastUpdated: new FormControl('')
    });
    this.loadData();
  }
  loadData() {
    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    var id = idParam ? +idParam : 0;
    // fetch the product from the server
    var url = environment.baseUrl + 'api/Products/' + id;
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
  onSubmit() {
    var product = this.product;
    if (product) {
      product.name = this.form.controls['name'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      product.isActive = this.form.controls['isActive'].value === 'true';
      product.dateAdded = new Date(this.form.controls['dateAdded'].value);
      product.lastUpdated = new Date(this.form.controls['lastUpdated'].value);

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
  }
}
