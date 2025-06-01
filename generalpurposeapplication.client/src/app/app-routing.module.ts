import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HealthCheckComponent } from './health-check/health-check.component';
import { ProductsComponent } from './products/products.component';
import { ProductEditComponent } from './products/product-edit.component';
import { CategoriesComponent } from './categories/categories.component';
import { CategoryEditComponent } from './categories/category-edit.component';
import { LoginComponent } from './auth/login.component';

import { AuthGuard } from './auth/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'health-check', component: HealthCheckComponent },
  { path: 'products', component: ProductsComponent },
  { path: 'product/:id', component: ProductEditComponent, canActivate: [AuthGuard] },
  { path: 'product', component: ProductEditComponent, canActivate: [AuthGuard] },
  { path: 'categories', component: CategoriesComponent },
  { path: 'category/:id', component: CategoryEditComponent, canActivate: [AuthGuard] },
  { path: 'category', component: CategoryEditComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
