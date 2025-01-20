import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  //Angular signals gebruiken om een winkelwagen op te slaan
  cart = signal<Cart | null>(null);
  selectedDelivery = signal<DeliveryMethod | null>(null);

  //Angular computed signal gebruiken om het aantal producten weer te geven bij het winkelwagen icoon
  itemCount = computed(() => {
    return this.cart()?.items.reduce((som, item) => som + item.hoeveelheid, 0);
  });

  //Angular computed signal gebruiken om de verschillende tussenuitkomsten te berekenen en de totale kostprijs te berekenen
  totals = computed(() => {
    const cart = this.cart();
    const delivery = this.selectedDelivery();
    if(!cart) return null;
    const subtotaal = cart.items.reduce((som, item) => som + (item.prijs * item.hoeveelheid), 0);
    const verzending = delivery ? delivery.prijs : 0;
    const korting = 0;
    return {
      subtotaal,
      verzending,
      korting,
      totaal: subtotaal + verzending - korting
    }
  });

  //één bepaalde winkelwagen opvragen uit de database aan de hand van een meegegeven id
  getCart(id: string){
    return this.http.get<Cart>(this.baseUrl + 'cart?id=' + id).pipe(
      map(cart => {
        this.cart.set(cart);
        return cart;
      })
    )
  }

  //een winkelwagen aanmaken in de database
  setCart(cart: Cart){
    return this.http.post<Cart>(this.baseUrl + 'cart', cart).subscribe({
      next: cart => this.cart.set(cart)
    });
  }

  //een item en hoeveelheid toevoegen aan een winkelwagen
  addItemToCart(item: CartItem | Product, hoeveelheid = 1){
    const cart = this.cart() ?? this.createCart();

    if(this.isProduct(item)){
      item = this.mapProductToCartItem(item);
    }

    cart.items = this.addOrUpdateItem(cart.items, item, hoeveelheid);
    this.setCart(cart);
  }

  //één bepaald item of hoeveelheid verwijderen van het winkelwagentje
  //als er geen producten meer overblijven (leeg winkelwagentje) dan wordt het gehele wagentje verwijderd uit Redis database en uit localStorage (this.deleteCart)
  removeItemFromCart(productId: number, hoeveelheid = 1) {
    const cart = this.cart();

    if(!cart) return;

    const index = cart.items.findIndex(x => x.productId === productId);

    if(index !== -1){
      if(cart.items[index].hoeveelheid > hoeveelheid) {
        cart.items[index].hoeveelheid -= hoeveelheid;
      }
      else {
        cart.items.splice(index, 1);
      }

      if(cart.items.length === 0){
        this.deleteCart();
      }
      else{
        this.setCart(cart);
      }
    }
  }

  //winkelwagen verwijderen uit de Redis database
  deleteCart() {
    this.http.delete(this.baseUrl + 'cart?id=' + this.cart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      }
    })
  }

  //item toevoegen of de hoeveelheid ervan wijzigen
  private addOrUpdateItem(items: CartItem[], item: CartItem, hoeveelheid: number): CartItem[] {
    const index = items.findIndex(x => x.productId === item.productId);
    if(index === -1) {
      item.hoeveelheid = hoeveelheid;
      items.push(item);
    } else {
      items[index].hoeveelheid += hoeveelheid
    }

    return items;
  }
  
  //product omvormen tot cartItem
  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productNaam: item.naam,
      prijs: item.prijs,
      hoeveelheid: 0,
      fotoUrl: item.fotoURL,
      merk: item.merk,
      type: item.type
    }
  }

  //controleren of een item een product of een cardItem is
  private isProduct(item: CartItem | Product): item is Product {
    return (item as Product).id !== undefined;
  }

  //een nieuwe winkelwagen aanmaken in de localStorage
  private createCart(): Cart {
    const cart = new Cart();

    localStorage.setItem('cart_id', cart.id);
    return cart;
  }
}
