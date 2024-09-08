
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product!: Product;
  productId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProductDetails();
  }

  loadProductDetails(): void {
    this.productService.getProductById(this.productId).subscribe({
      next: (product) => this.product = product,
      error: (err) => console.error('Error loading product details:', err)
    });
  }

  addToCart(): void {
    if (this.product) {
      this.cartService.addToCart(this.product);
      alert(`${this.product.name} added to cart!`);
    }
  }

  cancel(): void {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}