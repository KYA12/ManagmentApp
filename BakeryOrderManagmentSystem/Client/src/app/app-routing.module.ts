import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './pages/home/home.component';
import { ProductDetailsComponent } from './pages/product-details/product-details.component';
import { CartPageComponent } from './pages/cart/cart-page.component';
import { OrderTrackingComponent } from './pages/order-tracking/order-tracking.component';
import { ProductFormComponent } from './pages/product-form/product-form.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'cart', component: CartPageComponent },
  { path: 'order-tracking', component: OrderTrackingComponent },
  { path: 'product/new', component: ProductFormComponent },
  { path: 'product/edit/:id', component: ProductFormComponent },
  { path: 'product/:id', component: ProductDetailsComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }