import { Route } from "@angular/router";
import { adminGuard } from "../../core/guards/admin.guard";
import { authGuard } from "../../core/guards/auth.guard";
import { AdminComponent } from "./admin.component";

export const adminRoutes: Route[] = [
    {path: '', component: AdminComponent, canActivate: [authGuard, adminGuard]}
]