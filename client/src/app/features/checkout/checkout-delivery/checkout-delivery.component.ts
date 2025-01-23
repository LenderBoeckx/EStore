import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';
import { firstValueFrom } from 'rxjs';


@Component({
  selector: 'app-checkout-delivery',
  standalone: true,
  imports: [
    MatRadioModule,
    CurrencyPipe
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutService = inject(CheckoutService);
  cartService = inject(CartService);
  deliveryComplete = output<boolean>();

  ngOnInit(): void {
    //controleren of er eerder al een leveringsmethode geselecteerd is door de gebruiker en deze ophalen
    this.checkoutService.getDeliveryMethods().subscribe({
      next: methods => {
        if(this.cartService.cart()?.deliveryMethodId) {
          const method = methods.find(x => x.id === this.cartService.cart()?.deliveryMethodId)
          if(method) {
            this.cartService.selectedDelivery.set(method);
            this.deliveryComplete.emit(true);
          }
        }
      }
    });
  }

  //als de leveringsmethode aangepast is moet ook de winkelwagen aangepast worden met de geselecteerde levering
  async updateDeliveryMethod(method: DeliveryMethod){
    //de geselecteerde leveringsmethode defineëren in de cart service met een set functie voor de selectedDelivery signal
    this.cartService.selectedDelivery.set(method);
    //winkelwagen zoeken in de cart service
    const cart = this.cartService.cart();

    //als de winkelwagen gevonden is, de leveringsmethode aanpassen in de winkelwagen
    if(cart) {
      cart.deliveryMethodId = method.id;
      await firstValueFrom(this.cartService.setCart(cart));
      this.deliveryComplete.emit(true);
    }
  }
}
