import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { CartService } from '../services/cart.service';
import { SnackbarService } from '../services/snackbar.service';

export const emptyCardGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const snackService = inject(SnackbarService);

  if(!cartService.cart() || cartService.cart()?.items.length === 0){
    snackService.error("Er zijn geen artikelen in uw winkelmandje.");
    return false;
  }
  return true;
};
