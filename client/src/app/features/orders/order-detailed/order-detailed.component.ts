import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from "../../../shared/pipes/address.pipe";
import { PaymentPipe } from "../../../shared/pipes/payment.pipe";
import { AccountService } from '../../../core/services/account.service';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-order-detailed',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    PaymentPipe,
    RouterLink
],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private adminService = inject(AdminService);
  order?: Order;
  buttonText = this.accountService.isAdmin() ? 'Terug naar admin pagina' : 'Terug naar de bestellingen';
 
  ngOnInit(): void {
      this.loadOrder();
  }

  onReturnClick() {
    this.accountService.isAdmin() ? this.router.navigateByUrl('/admin') : this.router.navigateByUrl('/orders');
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if(!id) return;

    const loadOrderData = this.accountService.isAdmin() ? this.adminService.getOrder(+id) : this.orderService.getOrderDetail(+id);

    loadOrderData.subscribe({
      next: order => this.order = order
    });
  }
}
