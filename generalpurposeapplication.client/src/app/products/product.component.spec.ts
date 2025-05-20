import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { ProductsComponent } from './products.component';
import { Product } from './product';
import { ProductService } from './product.service';
import { ApiResult } from '../base.service';

describe('ProductsComponent', () => {
  let component: ProductsComponent;
  let fixture: ComponentFixture<ProductsComponent>;
  beforeEach(async () => {
    // Create a mock productService object with a mock 'getData' method
    let productService = jasmine.createSpyObj<ProductService>('ProductService',
      ['getData']);
    // Configure the 'getData' spy method
    productService.getData.and.returnValue(
      // return an Observable with some test data
      of<ApiResult<Product>>(<ApiResult<Product>>{
        data: [
          <Product>{
            name: 'TestProduct1',
            id: 1, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          },
          <Product>{
            name: 'TestProduct2',
            id: 2, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          },
          <Product>{
            name: 'TestProduct3',
            id: 3, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      }));
    await TestBed.configureTestingModule({
      declarations: [ProductsComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        {
          provide: ProductService,
          useValue: productService
        }
      ]
    })
      .compileComponents();
  });
  beforeEach(() => {
    fixture = TestBed.createComponent(ProductsComponent);
    component = fixture.componentInstance;
    component.paginator = jasmine.createSpyObj(
      "MatPaginator", ["length", "pageIndex", "pageSize"]
    );
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should display a "Products" title', () => {
    let title = fixture.nativeElement
      .querySelector('h1');
    expect(title.textContent).toEqual('Products');
  });

  it('should contain a table with a list of one or more Products', () => {
    let table = fixture.nativeElement
      .querySelector('table.mat-mdc-table');
    let tableRows = table
      .querySelectorAll('tr.mat-mdc-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
