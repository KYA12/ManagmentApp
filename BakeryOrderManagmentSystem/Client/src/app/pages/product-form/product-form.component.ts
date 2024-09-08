import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  isEditMode: boolean = false;
  productId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.productId = params['id'] ? +params['id'] : null;

      if (this.productId) {
        this.isEditMode = true;
        this.loadProduct(this.productId);
      }
    });
  }

  loadProduct(productId: number): void {
    this.productService.getProductById(productId).subscribe((product: Product) => {
      this.productForm.patchValue(product);
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      const productData = this.productForm.value;
      productData.isActive = true;

      if (this.isEditMode) {
        this.productService.updateProduct(this.productId!, productData).subscribe(() => {
          this.router.navigate(['/']);
        });
      } else {
        this.productService.addProduct(productData).subscribe(() => {
          this.router.navigate(['/']);
        });
      }
    }
  }

  cancel(): void {
    this.router.navigate(['/']);
  }
}