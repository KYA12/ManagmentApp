import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HomeComponent } from '../../pages/home/home.component';
import { ProductService } from '../../core/services/product.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FilterPipe } from '../../core/pipes/filter.pipe';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card'; 

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let productService: ProductService;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeComponent, FilterPipe ],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatCardModule, 
        FormsModule,
        BrowserAnimationsModule
      ],
      providers: [
        {
          provide: ProductService,
          useValue: {
            getActiveProducts: () => of([
              { productId: 1, name: 'Product 1', description: 'Description 1', price: 10 },
              { productId: 2, name: 'Product 2', description: 'Description 2', price: 20 }
            ]),
            deleteProduct: (id: number) => of(null)
          }
        }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    productService = TestBed.inject(ProductService);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load products on init', () => {
    component.ngOnInit();
    fixture.detectChanges();
    expect(component.products.length).toBe(2);
  });

  it('should navigate to product creation page on createProduct call', () => {
    spyOn(router, 'navigate');
    component.createProduct();
    expect(router.navigate).toHaveBeenCalledWith(['/product/new']);
  });

  it('should navigate to product edit page on editProduct call', () => {
    spyOn(router, 'navigate');
    component.editProduct(1);
    expect(router.navigate).toHaveBeenCalledWith(['/product/edit/1']);
  });

  it('should delete product and reload products on deleteProduct call', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(productService, 'deleteProduct').and.callThrough();
    spyOn(component, 'loadProducts').and.callThrough();
    component.deleteProduct(1);
    expect(productService.deleteProduct).toHaveBeenCalledWith(1);
    expect(component.loadProducts).toHaveBeenCalled();
  });
});