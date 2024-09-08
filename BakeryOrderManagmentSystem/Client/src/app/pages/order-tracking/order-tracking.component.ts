import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../core/models/order.model';
import { OrderStatus } from '../../core/models/order-status.model';

@Component({
  selector: 'app-order-tracking',
  templateUrl: './order-tracking.component.html',
  styleUrls: ['./order-tracking.component.scss']
})
export class OrderTrackingComponent implements OnInit {
  orders: Order[] = [];
  selectedOrder: Order | null = null;
  errorMessage: string = '';
  orderStatuses = Object.values(OrderStatus); 

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getOrders().subscribe({
      next: (orders) => { this.orders = orders; } ,
      error: (err) => this.errorMessage = 'Error loading orders: ' + err.message
    });
  }

  viewOrder(orderId: number): void {
    this.orderService.getOrderById(orderId).subscribe({
      next: (order) => this.selectedOrder = order,
      error: (err) => this.errorMessage = 'Error loading order details: ' + err.message
    });
  }

  updateOrderStatus(orderId: number, status: OrderStatus): void {
    console.log(status);
    this.orderService.updateOrderStatus(orderId, status).subscribe({
      next: () => {
        this.selectedOrder = null;
        this.loadOrders();
      },
      error: (err) => this.errorMessage = 'Error updating order status: ' + err.message
    });
  }

  clearSelection(): void {
    this.selectedOrder = null;
  }

  deleteOrder(orderId: number): void {
    if (confirm('Are you sure you want to delete this order?')) {
      this.orderService.deleteOrder(orderId).subscribe({
        next: () => {
          this.selectedOrder = null;
          this.loadOrders();
        },
        error: (err) => this.errorMessage = 'Error deleting order: ' + err.message
      });
    }
  }
}