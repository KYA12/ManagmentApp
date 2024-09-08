import { OrderProduct } from './order-product.model';
import { OrderStatus } from './order-status.model';

export interface Order {
  orderId: number;
  customerId: number;
  orderDate: Date;
  status: OrderStatus;
  ordersProducts: OrderProduct[];
}