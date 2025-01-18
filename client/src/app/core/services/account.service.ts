import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Address, User } from '../../shared/models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  //http request naar back-end login endpoint sturen om een user in te loggen
  login(values: any){
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http.post<User>(this.baseUrl + 'login', values, {params});
  }

  //http request naar back-end register endpoint sturen om een user te registreren
  register(values: any){
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  //request naar back-end user-info endpoint sturen om de gegevens van deze bepaalde user op te halen
  //hiermee controleren dat de user ingelogd is
  //cookie is niet leesbaar vanuit angular (gemarkeerd als httpOnly), dus deze manier toepassen voor de controle op inloggen
  getUserInfo(){
    return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
      map(user => {
        this.currentUser.set(user);
        return user;
      })
    )
  }

  //request naar back-end logout endpoint sturen om deze bepaalde user uit te loggen
  logout(){
    return this.http.post(this.baseUrl + 'account/logout', {});
  }

  //request naar back-end address endpoint sturen om het adres van deze bepaalde user te wijzigen
  updateAddress(address: Address){
    return this.http.post(this.baseUrl + 'account/address', address);
  }

  //request naar auth-status back-end endpoint sturen om te kijken of een user authenticated is, geeft een true of false terug
  getAuthState(){
    return this.http.get<{isAuthenticated: boolean}>(this.baseUrl + 'account/auth-status');
  }
}
