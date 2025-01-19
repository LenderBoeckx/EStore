import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { StripeAddressElement } from '@stripe/stripe-js';
import { Cart } from '../../shared/models/cart';
import { SnackbarService } from '../../core/services/snackbar.service';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButtonModule,
    RouterLink,
    MatCheckboxModule
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private accountService = inject(AccountService);
  addressElement?: StripeAddressElement;
  private snackbar = inject(SnackbarService);
  saveAddress = false;

  async ngOnInit() {
    //proberen om de adres element aan te maken in de stripe service
    //adres element zichtbaar maken in de template component
    try{
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
    } catch(error: any){
      //in het geval dat er errors zijn, de snackbar service aanspreken om een error boodschap onderaan het scherm te tonen
      //deze error boodschappen zijn gedefineërd in de stripe service
      this.snackbar.error(error.message);
    }
  }
  
  async onStepChange(event: StepperSelectionEvent) {
    if(event.selectedIndex === 1) {
      if(this.saveAddress) {
        const address = await this.getAddressFromStripeAddress();
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }
  }

  ngOnDestroy(): void {
    //beide elements via de stripe service terug op undefined zetten
    this.stripeService.disposeElements();
  }

  //saveAddress variabele een waarde geven als de checked status veranderd
  onSaveAddressCheckboxChange(event: MatCheckboxChange){
    this.saveAddress = event.checked;
  }

  //ee, opgeslagen adres uit het adres element halen en terugsturen
  private async getAddressFromStripeAddress(): Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if(address) {
      return {
        straat: address.line1,
        toevoeging: address.line2 || undefined,
        plaats: address.city,
        provincie: address.state,
        postcode: address.postal_code,
        land: address.country
      };
    } else return null;
  }
}
