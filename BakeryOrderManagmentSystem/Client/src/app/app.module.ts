import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { ProductDetailsComponent } from './pages/product-details/product-details.component';
import { CartPageComponent } from './pages/cart/cart-page.component';
import { OrderTrackingComponent } from './pages/order-tracking/order-tracking.component';
import { ProductCardComponent } from './shared/components/product-card/product-card.component';
import { ProductFormComponent } from './pages/product-form/product-form.component';

import { ProductService } from './core/services/product.service';
import { OrderService } from './core/services/order.service';
import { CartService } from './core/services/cart.service';

// Import Pipes
import { FilterPipe } from './core/pipes/filter.pipe';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ProductDetailsComponent,
    CartPageComponent,
    OrderTrackingComponent,
    ProductCardComponent,
    ProductFormComponent,
    FilterPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatInputModule,
    MatListModule,  
    MatIconModule,
    MatDialogModule,
    MatSelectModule, 
    MatOptionModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    ProductService,
    OrderService,
    CartService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }