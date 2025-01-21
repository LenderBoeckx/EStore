import { Component, inject, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../../core/services/signalr.service';
import { MatProgressSpinnerModule, MatSpinner } from '@angular/material/progress-spinner';
import { CurrencyPipe, DatePipe, NgIf } from '@angular/common';
import { PaymentPipe } from '../../../shared/pipes/payment.pipe';
import { AddressPipe } from '../../../shared/pipes/address.pipe';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [
    MatButtonModule,
    RouterLink,
    MatProgressSpinnerModule,
    DatePipe,
    PaymentPipe,
    CurrencyPipe,
    AddressPipe,
    NgIf
  ],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.scss'
})
export class CheckoutSuccessComponent implements OnDestroy {
  signalRService = inject(SignalrService);
  private orderService = inject(OrderService);

  ngOnDestroy(): void {
    //bij het verlaten van de succes pagina, wordt de orderComplete op false gezet zodat de gebruiker niet meer terug naar deze pagina kan navigeren
    this.orderService.orderComplete = false;
    //order uit de actieve signal halen door deze op terug op null te zetten
    this.signalRService.orderSignal.set(null);
  }
}
