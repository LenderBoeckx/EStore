import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
    baseUrl = 'https://localhost:5001/api/';
    private http = inject(HttpClient);
    types: String[] = [];
    merken: String[] = [];

  //producten ophalen uit de database
  getProducts(shopParams: ShopParams){
    let params = new HttpParams();


    if(shopParams.merken.length > 0) {
      params = params.append('merken', shopParams.merken.join(','));
    }
    if(shopParams.types.length > 0) {
      params = params.append('types', shopParams.types.join(','));
    }
    if(shopParams.sort){
      params = params.append('sort', shopParams.sort);
    }
    if(shopParams.search){
      params = params.append('search', shopParams.search);
    }

    params = params.append('pageSize', shopParams.pageSize);

    params = params.append('pageIndex', shopParams.pageNumber);

    return this.http.get<Pagination<Product>>(this.baseUrl + 'products', {params});
  }

  //details bekijken van een gekozen product
  getProduct(id: number){
    return this.http.get<Product>(this.baseUrl + 'products/' + id);
  }

  //merken ophalen uit de database
  getMerken(){
    if(this.merken.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/merken').subscribe({
      next: response => this.merken = response
    });
  }

  //types ophalen uit de database
  getTypes(){
    if(this.types.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/types').subscribe({
      next: response => this.types = response
    });
  }
}
