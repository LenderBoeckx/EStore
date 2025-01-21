import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { LeveringsAdres } from '../models/order';

@Pipe({
  name: 'address',
  standalone: true
})
export class AddressPipe implements PipeTransform {

  //object gegevens van het object shipping en leveringsAdres omvormen naar een tekstuele weergave
  transform(value?: ConfirmationToken['shipping'] | LeveringsAdres, ...args: unknown[]): unknown {
    if(value && 'address' in value && value.name) {
      const {line1, line2, city, country, postal_code} = (value as ConfirmationToken['shipping'])?.address!;
      return `${value.name}, ${line1}${line2 ? ', ' + line2 : ''}, ${city}, ${postal_code}, ${country}`;
    } else if(value && 'straat' in value){
      const {straat, toevoeging, plaats, land, postcode} = value as LeveringsAdres;
      return `${straat}${toevoeging ? ', ' + toevoeging: ''}, ${plaats}, ${postcode}, ${land}`;
    } else {
      return 'Ongekend adres.';
    }
  }

}
