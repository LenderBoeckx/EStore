import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if(accountService.currentUser()){
    return of(true);
  } else {
    return accountService.getAuthState().pipe(
      map(auth => {
        //controleren of de isAuthenticated true is, in dat geval de user doorsturen naar de gewenste bestemming
        if(auth.isAuthenticated) {
          return true;
        } else {
          //als de isAuthenticated false is dan wordt de user doorgestuurd naar de login component
          router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false;
        }
      })
    );
  }
};
