import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';
import { CartItem } from '../../core/models/cart-item.model';
import { Order } from '../../core/models/order.model';
import { OrderProduct } from '../../core/models/order-product.model';
import { OrderStatus } from '../../core/models/order-status.model';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss']
})
export class CartPageComponent implements OnInit {
  cartItems: CartItem[] = [];
  totalCost: number = 0;
  order!: Order;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartItems = this.cartService.getCartItems();
    this.calculateTotal();
  }

  removeFromCart(productId: number): void {
    this.cartService.removeFromCart(productId);
    this.cartItems = this.cartService.getCartItems();
    this.calculateTotal();
  }

  cancel(): void {
    this.router.navigate(['/']);
  }

  private calculateTotal(): void {
    this.totalCost = this.cartItems.reduce((sum, item) => sum + (item.product.price * item.quantity), 0);
  }

  placeOrder(): void {
    if (this.cartItems.length === 0) {
      alert('Cannot place an order. The cart is empty.');
      return;
    }

    const orderProducts: OrderProduct[] = this.cartItems.map(item => ({
      orderProductId: 0,
      orderId: 0,
      productId: item.product.productId,
      quantity: item.quantity,
      productName: item.product.name
    }));

    const newOrder: Order = {
      orderId: 0,
      customerId: 1,
      orderDate: new Date(),
      status: OrderStatus.Pending,
      ordersProducts: orderProducts
    };
    console.log(newOrder);
    this.orderService.placeOrder(newOrder).subscribe({
      next: (order) => {
        this.clearCart();
        this.router.navigate(['/order-tracking'], { queryParams: { orderId: order.orderId } });
      },
      error: (error) => {
        console.error('Failed to create order', error);
      }
    });
  }

  clearCart(): void {
    this.cartService.clearCart();
    this.cartItems = [];
    this.totalCost = 0;
  }
}
