import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from "../../shared/components/order-summary/order-summary.component";
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import { ConfirmationToken, StripeAddressElement, StripeAddressElementChangeEvent, StripePaymentElement, StripePaymentElementChangeEvent } from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../core/services/account.service';
import { CheckoutDeliveryComponent } from "./checkout-delivery/checkout-delivery.component";
import { CheckoutReviewComponent } from "./checkout-review/checkout-review.component";
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LeveringsAdres, OrderToCreate } from '../../shared/models/order';
import { OrderService } from '../../core/services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButtonModule,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
    MatProgressSpinnerModule
],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private accountService = inject(AccountService);
  cartService = inject(CartService);
  private router = inject(Router);
  private orderService = inject(OrderService);
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  private snackbar = inject(SnackbarService);
  saveAddress = false;
  completionStatus = signal<{address: boolean, payment: boolean, delivery: boolean}>({address: false, payment: false, delivery: false});
  confirmationToken?: ConfirmationToken;
  loading = false;

  async ngOnInit() {
    //proberen om de adres en payment element aan te maken in de stripe service
    //adres en payment element zichtbaar maken in de template component
    try{
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
      this.addressElement.on('change', this.handleAddressChange);
      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change', this.handlePaymentChange);
    } catch(error: any){
      //in het geval dat er errors zijn, de snackbar service aanspreken om een error boodschap onderaan het scherm te tonen
      //deze error boodschappen zijn gedefineërd in de stripe service
      this.snackbar.error(error.message);
    }
  }

  //de adres property van de signal completionStatus op true zetten zodat de gebruiker naar een volgende stap kan navigeren
  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.address = event.complete;
      return state;
    });
  }

  //de card property van de signal completionStatus op true zetten zodat de gebruiker naar een volgende stap kan navigeren
  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update(state => {
      state.payment = event.complete;
      return state;
    });
  }
  
  //de card property van de signal completionStatus op true zetten zodat de gebruiker naar een volgende stap kan navigeren
  handleDeliveryChange(event: boolean) {
    this.completionStatus.update(state => {
      state.delivery = event;
      return state;
    });
  }

  //stripe service aanspreken om een confirmation token aan te maken bij stripe
  //wordt enkel uitgevoerd als de completionStatus volledig voldaan is (alle waarden geven true)
  async getConfirmationToken() {
    try{
      if(Object.values(this.completionStatus()).every(status => status === true)) {
        const result = await this.stripeService.createConfirmationToken();
        if(result.error) throw new Error(result.error.message);
        this.confirmationToken = result.confirmationToken;
        console.log(this.confirmationToken);
      }
    } catch(error: any) {
      this.snackbar.error(error.message);
    }
    
  }

  async onStepChange(event: StepperSelectionEvent) {
    //controleren dat de stepper naar de tweede pagina gaat (van index 0 naar index 1)
    if(event.selectedIndex === 1) {
      //controleren dat de saveAddress variabele true is
      if(this.saveAddress) {
        //adres ophalen vanuit het stripe adres menu in de template
        const address = await this.getAddressFromStripeAddress() as Address;
        //account service opdracht geven om het adres aan te passen in de database
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }
    //controleren dat de stepper naar de derde pagina gaat (van index 1 naar index 2)
    if(event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentItent());
    }
    //controleren dat de stepper naar de laatste pagina gaat (van index 2 naar index 3)
    if(event.selectedIndex === 3) {
      //opdracht geven aan de methode getConfirmationToken om een confirmationtoken op te halen
      await this.getConfirmationToken();
    }
  }

  //stripe service opdracht geven om na te gaan of de betalen voldaan is en het bestelde order plaatsen in de database
  async confirmPayment(stepper: MatStepperModule) {
    this.loading = true;
    try {
      if(this.confirmationToken){
        const result = await this.stripeService.confirmPayment(this.confirmationToken);

        if(result.paymentIntent?.status === 'succeeded') {
          //als de betaling voldaan is volgens Stripe, dan kan het order aangemaakt worden
          const order = await this.createOrderModel();
          console.log(order);
          //order service de opdracht geven om het order aan te maken in de database
          const orderResult = await firstValueFrom(this.orderService.createOrder(order));
          console.log('orderResult');
          console.log(orderResult);

          if(orderResult) {
            //winkelwagentje verwijderen en geselecteerde leveringsmethode op null zetten + gebruiker navigeren naar succes component
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('/checkout/success');
          } else {
            throw new Error('Het aanmaken van de bestelling is mislukt.');
          }
        } else if(result.error) {
          throw new Error(result.error.message);
        } else {
          throw new Error('Er is iets mis gegaan.');
        }
      }
    } catch (error: any) {
      this.snackbar.error(error.message || 'Er is iets mis gegaan.');
    } finally {
      this.loading = false;
    }
  }

  //Ordermodel aanmaken om op te slaan in de database
  private async createOrderModel(): Promise<OrderToCreate> {
    const cart = this.cartService.cart();
    const shippingAddress = await this.getAddressFromStripeAddress() as LeveringsAdres;
    const card = this.confirmationToken?.payment_method_preview.card;

    if(!cart?.id || !cart?.deliveryMethodId || !card || !shippingAddress) {
      throw new Error("Probleem bij het aanmaken van de bestelling.");
    }

    return {
      winkelwagenId: cart.id,
      betalingsOverzicht: {
        last4: +card.last4,
        brand: card.brand,
        expMonth: card.exp_month,
        expYear: card.exp_year
      },
      leveringsMethodeId: cart.deliveryMethodId,
      leveringsAdres: shippingAddress
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

  //een opgeslagen adres uit het adres element halen en terugsturen
  private async getAddressFromStripeAddress(): Promise<Address | LeveringsAdres | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if(address) {
      return {
        naam: result.value.name,
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
