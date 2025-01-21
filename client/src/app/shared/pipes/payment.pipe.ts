import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { BetalingsOverzicht } from '../models/order';

@Pipe({
  name: 'payment',
  standalone: true
})
export class PaymentPipe implements PipeTransform {

  //object gegevens van het object payment_method_preview vanuit het confirmationToken en betalingsOverzicht omvormen naar een tekstuele weergave
  transform(value?: ConfirmationToken['payment_method_preview'] | BetalingsOverzicht, ...args: unknown[]): unknown {
    if(value && 'card' in value) {
      const {brand, last4, exp_month, exp_year} = (value as ConfirmationToken['payment_method_preview']).card!;
      return `${brand.toUpperCase()}, **** **** **** ${last4}, Exp: ${exp_month}/${exp_year}`;
    } else if(value && 'last4' in value) {
      const {brand, last4, expMonth, expYear} = value as BetalingsOverzicht;
      return `${brand.toUpperCase()}, **** **** **** ${last4}, Exp: ${expMonth}/${expYear}`;
    } else {
      return 'Ongekende betalingsmethode.';
    }
  }

}
