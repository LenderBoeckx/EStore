import { inject, Injectable } from '@angular/core';
import {ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private cartService = inject(CartService);
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private stripePromise?: Promise<Stripe | null>;
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElment?: StripePaymentElement;

  constructor() {
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {
    return this.stripePromise;
  }

  //elements van type StrypeElements van de juiste parameters voorzien
  //clientsecret en de appearance
  async initializeElements() {
    if(!this.elements) {
      const stripe = await this.getStripeInstance();
      if(stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentItent());
        this.elements = stripe.elements({clientSecret: cart.clientSecret, appearance: {labels: 'floating'}});
      }
      else{
        throw new Error('Stripe kon niet geladen worden.');
      }
    }
    return this.elements;
  }

  //adres element van stripe opvullen met de juiste options, in dit geval de mode shipping als er nog geen adres element is aangemaakt en de initialize element voldaan is
  async createAddressElement() {
    if(!this.addressElement){
      const elements = await this.initializeElements();
      if(elements) {
        //ingelogde gebruiker ophalen aan de hand van de account service
        //nieuwe variabele maken defaultvalus van het type 'StripeAddressElementOptions['defaultValues']', deze verder opvullen doorheen de methode
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        //als de gebruiker gevonden is
        //de volledige naam meegeven als default value van naam
        if(user){
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }
        //controleren of de gebruiker eerder al een adres heeft ingevuld
        //in geval van wel, dan de defaultValues invullen met de gevonden gegevens in de database
        if(user?.address){
          defaultValues.address = {
            line1: user.address.straat,
            line2: user.address.toevoeging,
            city: user.address.plaats,
            state: user.address.provincie,
            postal_code: user.address.postcode,
            country: user.address.land
          }
        }
        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Element instantie kon niet geladen worden.');
      }
    }
    return this.addressElement;
  }

  //winkelwagen aanpassen en ophalen, deze functie geeft de gewijzigde winkelwagen terug
  createOrUpdatePaymentItent() {
    const cart = this.cartService.cart();

    if(!cart) throw new Error('Probleem met winkelwagentje.');

    return this.http.post<Cart>(this.baseUrl + 'payment/' + cart.id, {}).pipe(
      map(cart => {
        this.cartService.setCart(cart);
        return cart;
      })
    );
  }

  //methode om een confirmation token aan te maken binnen stripe
  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();
    if(result.error) throw new Error(result.error.message);
    if(stripe) {
      return await stripe.createConfirmationToken({elements});
    } else {
      throw new Error('Stripe is niet beschikbaar');
    }
  }

  //methode om na te gaan bij stripe of de betaling voldaan is
  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();
    if(result.error) throw new Error(result.error.message);
    const clientSecret = this.cartService.cart()?.clientSecret;

    if(stripe && clientSecret){
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id
        },
        redirect: 'if_required'
      });
    } else {
      throw new Error('Stripe kon niet geladen worden.');
    }
  }

  //payment element van stripe opvullen met de juiste options
  async createPaymentElement() {
    //controleren of er al een payment element opgevuld is (niet undefined)
    if(!this.paymentElment) {
      const elements = await this.initializeElements();
      if(elements){
        //nieuw element 'payment' aanmaken
        this.paymentElment = elements.create('payment');
      } else {
        throw new Error('Element instantie kon niet geladen worden.');
      }
    }
    return this.paymentElment;
  }

  //methode om element terug op undefined te zetten
  //te gebruiken wanneer een gebruiker uitlogd
  disposeElements(){
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElment = undefined;
  }

}
