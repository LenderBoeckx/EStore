import { Route } from "@angular/router";
import { authGuard } from "../../core/guards/auth.guard";
import { emptyCardGuard } from "../../core/guards/empty-card.guard";
import { orderCompleteGuard } from "../../core/guards/order-complete.guard";
import { CheckoutSuccessComponent } from "./checkout-success/checkout-success.component";
import { CheckoutComponent } from "./checkout.component";

export const checkoutRoutes: Route[] = [
        {path: '', component: CheckoutComponent, canActivate: [authGuard, emptyCardGuard]},
        {path: 'success', component: CheckoutSuccessComponent, canActivate: [authGuard, orderCompleteGuard]},
]