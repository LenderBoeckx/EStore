import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe, Location, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { firstValueFrom } from 'rxjs';
import { StripeService } from '../../../core/services/stripe.service';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-order-summary',
  standalone: true,
  imports: [
    MatButtonModule,
    RouterLink,
    MatFormField,
    MatLabel,
    MatInput,
    CurrencyPipe,
    FormsModule,
    NgIf,
    MatIcon
  ],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss'
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
  location = inject(Location);
  stripeService = inject(StripeService);
  code?: string;

  applyCouponCode() {
    if(!this.code) return;
    this.cartService.applyDiscount(this.code).subscribe({
      next: async coupon => {
        const cart = this.cartService.cart();
        if(cart) {
          cart.coupon = coupon;
          await firstValueFrom(this.cartService.setCart(cart));
          this.code = undefined;
        }
        if(this.location.path() === '/checkout') {
          await firstValueFrom(this.stripeService.createOrUpdatePaymentItent());
        }
      }
    });
  }

  async removeCouponCode() {
    const cart = this.cartService.cart();

    if(!cart) return;

    if(cart.coupon) cart.coupon = undefined;
    await firstValueFrom(this.cartService.setCart(cart));

    if(this.location.path() === '/checkout') {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentItent());
    }
  }
}
