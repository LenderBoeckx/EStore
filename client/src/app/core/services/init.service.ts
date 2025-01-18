import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  init() {
    //cartId ophalen vanuit de localstorage
    const cartId = localStorage.getItem('cart_id');
    //winkelwagentje ophalen vanuit de Redis database met behulp van de getCart functie in de Cart service
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);

    //wachten op meerdere observables alvorens deze worden terug gestuurd
    return forkJoin({
      cart: cart$,
      //controleren of een user nog steeds ingelogd is, ook na her refreshen van een pagina
      user: this.accountService.getUserInfo()
    })
  }
}
