import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Category } from './category';
import { BaseFormComponent } from '../base-form.component';

@Component({
  selector: 'app-category-edit',
  templateUrl: './category-edit.component.html',
  styleUrl: './category-edit.component.scss'
})
export class CategoryEditComponent extends BaseFormComponent implements OnInit{
  // the view title
  title?: string;
  // the category object to edit or create
  category?: Category;
  // the category object id, as fetched from the active route:
  // It's NULL when we're adding a new category, and not NULL when we're editing an existing one.
  id?: number;
  // the categories array for the select
  categories?: Category[];
  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) {
    super();
  }
  ngOnInit() {
    this.form = this.fb.group({
      name: ['',
        Validators.required,
        this.isDupeField("name")
      ]
    });
    this.loadData();
  }
  loadData() {
    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE
      // fetch the category from the server
      var url = environment.baseUrl + "api/Categories/" + this.id;
      this.http.get<Category>(url).subscribe({
        next: (result) => {
          this.category = result;
          this.title = "Edit - " + this.category.name;
          // update the form with the category value
          this.form.patchValue(this.category);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Category";
    }
  }
  onSubmit() {
    var category = (this.id) ? this.category : <Category>{};
    if (category) {
      category.name = this.form.controls['name'].value;
      if (this.id) {
        // EDIT mode
        var url = environment.baseUrl + 'api/Categories/' + category.id;
        this.http
          .put<Category>(url, category)
          .subscribe({
            next: (result) => {
              console.log("Category " + category!.id + " has been updated.");
              // go back to categories view
              this.router.navigate(['/categories']);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        var url = environment.baseUrl + 'api/Categories';
        this.http
          .post<Category>(url, category)
          .subscribe({
            next: (result) => {
              console.log("Category " + result.id + " has been created.");
              // go back to categories view
              this.router.navigate(['/categories']);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }
  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{
      [key: string]: any
    } | null> => {
      var params = new HttpParams()
        .set("categoryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);
      var url = environment.baseUrl + 'api/Categories/IsDupeField';
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}
