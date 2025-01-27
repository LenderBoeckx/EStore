import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { NewProduct, Product } from '../../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  //een product toevoegen in de database
  createProduct(product: Partial<NewProduct>) {
    return this.http.post<Product>(this.baseUrl + 'products/', product);
  }
  //een product verwijderen uit de database
  deleteProduct(id: number) {
    return this.http.delete<void>(this.baseUrl + 'products/' + id);
  }
}
