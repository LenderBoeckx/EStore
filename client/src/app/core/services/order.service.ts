import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  //een order aanmaken in de database
  createOrder(orderToCreate: OrderToCreate) {
    return this.http.post<Order>(this.baseUrl + 'orders', orderToCreate);
  }

  //een lijst van alle orders voor een bepaalde gebruiker ophalen uit de database
  getOrdersForUser() {
    return this.http.get<Order[]>(this.baseUrl + 'orders');
  }

  //de details van een order uit de database ophalen aan de hand van het meegegeven id
  getOrderDetail(id: number) {
    return this.http.get<Order>(this.baseUrl + 'orders/' + id);
  }
}
