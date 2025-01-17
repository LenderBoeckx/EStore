import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { Location } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CurrencyPipe,
    MatButtonModule,
    MatIconModule,
    MatFormField,
    MatInputModule,
    MatLabel,
    MatDivider,
    FormsModule
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  cartService = inject(CartService);
  product?: Product;
  quantityInCart = 0;
  quantity = 1;

  constructor(private location: Location) {}

  ngOnInit(): void {
    this.loadProduct();
  }

  //product ophalen om te tonen in de detail component, dit gebeurd tijdens de init functie van Angular
  loadProduct(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if(!id) return;

    this.shopService.getProduct(+id).subscribe({
      next: product => {
        this.product = product;
        this.updateQuantityInCart();
      },
      error: error => console.log(error)
    });
  }

  //methode om de hoeveelheid in het winkelwagentje op te halen
  updateQuantityInCart(){
    this.quantityInCart = this.cartService.cart()?.items.find(x => x.productId === this.product?.id)?.hoeveelheid || 0;
    this.quantity = this.quantityInCart || 1;
  }

  //de cartService opdracht geven om items toe te voegen of te verwijderen uit de winkelwagen
  updateCart(){
    if(!this.product) return;
    if(this.quantity > this.quantityInCart) {
      const itemsToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart += itemsToAdd;
      this.cartService.addItemToCart(this.product, itemsToAdd);
    }
    else {
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -= itemsToRemove;
      this.cartService.removeItemFromCart(this.product.id, itemsToRemove);
    }

  }

  //nagaan welke tekst op de knop moet staan
  //als het product al in het winkelmandje aanwezig is dan moet de button 'winkelmandje aanpassen' weergeven
  //als het product nog niet in het winkelmandje aanwezig is dan moet de button 'toevoegen aan winkelmandje' weergeven
  getButtonText(){
    return this.quantityInCart > 0 ? 'Winkelmandje aanpassen' : 'Toevoegen aan winkelmandje';
  }

  //de pagina 1 stap terug in de browserhistory laten gaan
  goBack(){
    this.location.back();
  }
}
