import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  //verschillende fouten opvangen die door de server worden teruggestuurd
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if(err.status === 400) {
        //controleren of er meerdere errors in de array err.error aanwezig zijn (dit duidt op een validation error)
        if(err.error.errors) {
          const modelStateErrors = [];
          //itereren over de errors die in de array err.error.errors aanwezig zijn, vermoedelijk één of meerdere required velden niet ingevuld of foutieve data in een number veld
          //deze errors toevoegen aan de modelStateErrors array
          for(const key in err.error.errors) {
            if(err.error.errors[key]){
              modelStateErrors.push(err.error.errors[key]);
            }
          }
          //de modelStateErrors array terug sturen naar de component waar de error is veroorzaakt
          throw modelStateErrors.flat();
        } else {
          //snackbar tonen onderaan het scherm bij een bad request error
          snackbar.error(err.error.title || err.error);
        }
        
      }
      if(err.status === 401){
        //snackbar tonen onderaan het scherm bij een unauthorized error
        snackbar.error(err.error.title || err.error);
      }
      if(err.status === 403){
        //snackbar tonen onderaan het scherm bij een unauthorized error
        snackbar.error('Forbidden');
      }
      if(err.status === 404){
        //redirecten naar de not-found component
        router.navigateByUrl('/not-found');
      }
      if(err.status === 500){
        const navigationExtras: NavigationExtras = {state: {error: err.error}}
        //redirecten naar de server-error component
        router.navigateByUrl('/server-error', navigationExtras);
      }
      return throwError(() => err);
    })
  );
};
