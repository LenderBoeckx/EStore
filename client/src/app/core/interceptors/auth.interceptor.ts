import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  //request clonen
  //origineel request kan niet gemanipuleerd worden
  //de withCredentials toevoegen aan het gecloonde request
  const clonedRequest = req.clone({
    withCredentials: true
  });
  
  //gecloonde request terugsturen
  //op deze manier moet er niet aan elke request een withcredentials manueel toegevoegd worden
  return next(clonedRequest);
};
