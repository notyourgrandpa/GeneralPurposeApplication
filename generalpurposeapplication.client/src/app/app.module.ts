import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth/auth.interceptor';
import { NgModule, isDevMode } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AngularMaterialModule } from './shared/angular-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HealthCheckComponent } from './health-check/health-check.component';
import { ProductsComponent } from './products/components/product-list/products.component';
import { ProductEditComponent } from './products/components/product-edit/product-edit.component';
import { CategoriesComponent } from './categories/components/categories-list/categories.component';
import { CategoryEditComponent } from './categories/components/category-edit/category-edit.component';
import { LoginComponent } from './auth/login.component';

import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

import { ConnectionServiceModule } from 'ng-connection-service';
import { GraphQLModule } from './core/graphql.module';
import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';
import { InventoryLogsComponent } from './inventory-logs/components/inventory-logs-list/inventory-logs.component';
import { InventoryLogEditComponent } from './inventory-logs/components/inventory-log-edit/inventory-log-edit.component';
import { InventoryChangeTypePipe } from './shared/pipes/inventory-change-type.pipe';
import { SalesTransactionsComponent } from './sales-transactions/components/sales-transactions-list/sales-transactions.component';
import { SalesTransactionEditComponent } from './sales-transactions/components/sales-transaction-edit/sales-transaction-edit.component';
import { ProductDialogComponent } from './products/components/product-dialog/product-dialog.component';
import { ExpensesComponent } from './expenses/expenses-list/expenses.component';
import { ExpenseEditComponent } from './expenses/expense-edit/expense-edit.component';
import { ProductListPageComponent } from './products/components/product-list/product-list-page.component';
import { ProductListCoreComponent } from './products/components/product-list/product-list-core.component';
import { ProductListDialogComponent } from './products/components/product-list-dialog/product-list-dialog.component';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavMenuComponent,
    FetchDataComponent,
    HealthCheckComponent,
    ProductsComponent,
    ProductEditComponent,
    CategoriesComponent,
    CategoryEditComponent,
    LoginComponent,
    ConfirmDialogComponent,
    InventoryLogsComponent,
    InventoryLogEditComponent,
    InventoryChangeTypePipe,
    SalesTransactionsComponent,
    SalesTransactionEditComponent,
    ProductDialogComponent,
    ExpensesComponent,
    ExpenseEditComponent,
    ProductListPageComponent,
    ProductListCoreComponent,
    ProductListDialogComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    ReactiveFormsModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production,
      // Register the ServiceWorker as soon as the app is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    }),
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: !isDevMode(),
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    }),
    ConnectionServiceModule,
    GraphQLModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
