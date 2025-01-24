import { Directive, effect, inject, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]',
  standalone: true
})
export class IsAdminDirective {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  constructor() {
    effect(() => {
      //een directive voorzien om te kijken of een gebruiker ook admin is
      //als deze gebruiker admin is dan wordt er een view getoond met een link naar een admin gedeelte
      //bij een customer wordt deze link verborgen
      if(this.accountService.isAdmin()) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    })
   }
}
