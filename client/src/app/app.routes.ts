import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { CartComponent } from './features/cart/cart.component';


export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'shop', component: ShopComponent},
    {path: 'shop/:id', component: ProductDetailsComponent},
    {path: 'cart', component: CartComponent},
    {path: 'checkout', loadChildren: () => import('./features/checkout/routes').then(routes => routes.checkoutRoutes)},
    {path: 'orders', loadChildren: () => import('./features/orders/routes').then(routes => routes.orderRoutes)},
    {path: 'account', loadChildren: () => import('./features/account/routes').then(routes => routes.accountRoutes)},
    {path: 'test-error', loadChildren: () => import('./features/test-error/routes').then(routes => routes.errorRoutes)},
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: 'admin', loadChildren: () => import('./features/admin/routes').then(routes => routes.adminRoutes)},
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
