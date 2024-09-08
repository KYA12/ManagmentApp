import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models/product.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  products: Product[] = [];
  searchText: string = '';
  errorMessage: string = '';

  constructor(private productService: ProductService, private router: Router) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.productService.getActiveProducts().subscribe( { 
      next: (products) => { this.products = products; } ,
      error: (err) => this.errorMessage = 'Error loading products: ' + err.message
    });
  }

  createProduct(): void {
    this.router.navigate(['/product/new']);
  }

  editProduct(productId: number): void {
    this.router.navigate([`/product/edit/${productId}`]);
  }

  deleteProduct(productId: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(productId).subscribe(() => {
        this.loadProducts();
      });
    }
  }
}