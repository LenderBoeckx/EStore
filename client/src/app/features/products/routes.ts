import { Route } from "@angular/router";
import { CreateProductComponent } from "./create-product/create-product.component";
import { adminGuard } from "../../core/guards/admin.guard";
import { authGuard } from "../../core/guards/auth.guard";

export const productRoutes: Route[] = [
    {path: 'create-product', component: CreateProductComponent, canActivate: [authGuard, adminGuard]}
]