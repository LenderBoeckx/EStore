import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Order } from '../../shared/models/order';
import { AdminService } from '../../core/services/admin.service';
import { OrderParams } from '../../shared/models/orderParams';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatLabel, MatSelectChange, MatSelectModule } from '@angular/material/select';
import { CurrencyPipe, DatePipe, NgIf } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../core/services/dialog.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatLabel,
    MatTooltipModule,
    MatTabsModule,
    RouterLink
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  displayedColumns: string[] = ['id', 'koperEmail', 'bestellingsDatum', 'totaal', 'bestellingsStatus', 'action'];
  dataSource = new MatTableDataSource<Order>([]);
  private adminService = inject(AdminService);
  private dialogService = inject(DialogService);
  orderParams = new OrderParams();
  totalItems = 0;
  statusOptions = ['All', 'PaymentReceived', 'PaymentMismatch', 'Refunded', 'Pending'];

  //bij het initialiseren van de component dienen de orders geladen te worden
  ngOnInit(): void {
    this.loadOrders();
  }

  //de adminservice de opdracht geven om de bepaalde bestellingen op te halen die voldoen aan de meegegeven criteria
  loadOrders() {
    this.adminService.getOrders(this.orderParams).subscribe({
      next: response => {
        if(response.data) {
          this.dataSource.data = response.data;
          this.totalItems = response.count;
        }
      }
    })
  }

  //als de pagina in de table veranderd dan moeten de bijhorende bestellingen opgehaald worden
  onPageChange(event: PageEvent) {
    this.orderParams.pageNumber = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }

  //als de filter veranderd dan moeten ook de orders opgehaald worden die bij de gegeven filter horen
  onFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;
    this.loadOrders();
  }

  async openConfirmDialog(id: number) {
    const confirmed = await this.dialogService.confirm(
      'Bevestig terugbetaling',
      'Bent u zeker dat uw dit bedrag wil terugbetalen? Dit kan niet ongedaan gemaakt worden.'
    );

    if(confirmed) this.refundOrder(id);
  }

  refundOrder(id: number) {
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        this.dataSource.data = this.dataSource.data.map(o => o.id === id ? order : o)
      }
    });
  }
}
