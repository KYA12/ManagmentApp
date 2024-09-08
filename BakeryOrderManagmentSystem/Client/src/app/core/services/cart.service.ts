import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from '../models/product.model';
import { CartItem } from '../models/cart-item.model';
import { environment } from '../../../environments/environments.development';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems: CartItem[] = [];
  private cartItemsSubject = new BehaviorSubject<CartItem[]>(this.cartItems);

  cartItems$ = this.cartItemsSubject.asObservable();

  constructor() {
    this.loadCart();
  }

  addToCart(product: Product, quantity: number = 1): void {
    const existingItem = this.cartItems.find(item => item.product.productId === product.productId);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      this.cartItems.push({ product, quantity });
    }

    this.cartItemsSubject.next(this.cartItems);
    this.saveCart();
  }

  getCartItems(): CartItem[] {
    return this.cartItems;
  }

  getTotalCost(): number {
    return this.cartItems.reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  clearCart(): void {
    this.cartItems = [];
    this.cartItemsSubject.next(this.cartItems);
    this.saveCart();
  }

  private saveCart(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartItems));
  }

  private loadCart(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cartItems = JSON.parse(savedCart);
      this.cartItemsSubject.next(this.cartItems);
    }
  }

  removeFromCart(productId: number): void {
    this.cartItems = this.cartItems.filter(item => item.product.productId !== productId);
    this.cartItemsSubject.next(this.cartItems);
    this.saveCart();
  }
}