import { Route } from "@angular/router";
import { CreateProductComponent } from "./create-product/create-product.component";

export const productRoutes: Route[] = [
    {path: 'create-product', component: CreateProductComponent}
]