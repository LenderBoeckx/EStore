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
import { ShopService } from '../../core/services/shop.service';
import { ShopParams } from '../../shared/models/shopParams';
import { Product } from '../../shared/models/product';
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from '../shop/filters-dialog/filters-dialog.component';
import { ProductService } from '../../core/services/product.service';

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
  dataSourceOrders = new MatTableDataSource<Order>([]);
  dataSourceProducten = new MatTableDataSource<Product>([]);
  private adminService = inject(AdminService);
  private dialogService = inject(DialogService);
  shopService = inject(ShopService);
  private dialogServiceProducts = inject(MatDialog);
  private productService = inject(ProductService);
  orderParams = new OrderParams();
  shopParams = new ShopParams();
  totalOrders = 0;
  totalProducts = 0;
  statusOptions = ['All', 'PaymentReceived', 'PaymentMismatch', 'Refunded', 'Pending'];

  //bij het initialiseren van de component dienen de orders geladen te worden
  ngOnInit(): void {
    this.loadOrders();
    this.loadProducts();
    this.shopService.getMerken();
    this.shopService.getTypes();
  }


  //de adminservice de opdracht geven om de bepaalde bestellingen op te halen die voldoen aan de meegegeven criteria
  loadOrders() {
    this.adminService.getOrders(this.orderParams).subscribe({
      next: response => {
        if(response.data) {
          this.dataSourceOrders.data = response.data;
          this.totalOrders = response.count;
        }
      }
    });
  }

  //de productservice (shop) de opdracht geven om de bepaalde bestellingen op te halen die voldoen aan de meegegeven criteria
  loadProducts() {
    this.shopParams.sort = 'id';
    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => {
        if(response.data) {
          this.dataSourceProducten.data = response.data;
          this.totalProducts = response.count;
        }
      }
    });
  }

  //als de filter veranderd dan moeten ook de orders opgehaald worden die bij de gegeven filter horen
  onOrderFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;
    this.loadOrders();
  }

  //als de pagina in de table veranderd dan moeten de bijhorende bestellingen opgehaald worden
  onPageChange(event: PageEvent, tab: string) {
      if(tab === "Orders") {
        this.orderParams.pageNumber = event.pageIndex + 1;
        this.orderParams.pageSize = event.pageSize;
        this.loadOrders();
      }
      
    if(tab === "Producten") {
        this.shopParams.pageNumber = event.pageIndex + 1;
        this.shopParams.pageSize = event.pageSize;
        this.loadProducts();
    }
      
  }
  //dialoog doorgeven welke tekst er moet doorgegeven worden aan de delete dialoog bij order verwijderen
  openDeleteOrderDialog(id: number){
    const titel = 'Bevestig terugbetaling';
    const text = 'Bent u zeker dat uw dit bedrag wil terugbetalen? Dit kan niet ongedaan gemaakt worden.';

    this.openConfirmDialog(id, titel, text, 'order');
  }
  //dialoog doorgeven welke tekst er moet doorgegeven worden aan de delete dialoog bij product verwijderen
  openDeleteProductDialog(id: number) {
    const titel = 'Bevestig productverwijdering';
    const text = 'Bent u zeker dat u dit product wil verwijderen uit de database?';

    this.openConfirmDialog(id, titel, text, 'product');
  }
  
  //dialoog openen bij het klikken op verwijderen
  async openConfirmDialog(id: number, titel: string, text: string, type: string) {
    const confirmed = await this.dialogService.confirm(
      titel,
      text
    );

    if(type === 'order' && confirmed) this.refundOrder(id);

    if(type === 'product' && confirmed) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.loadProducts();
        }
      });
    } 
  }

  refundOrder(id: number) {
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        this.dataSourceOrders.data = this.dataSourceOrders.data.map(o => o.id === id ? order : o)
      }
    });
  }

  //functie getriggerd door een klik event op de knop filters
    openFiltersDialog(){
      const minWidth = window.innerWidth < 640 ? '90vw' : '500px';
      //dialog van material openen met de opties om merken en/of types te filteren
      const dialogRef = this.dialogServiceProducts.open(FiltersDialogComponent, {
        minWidth: minWidth,
        maxWidth: '95vw',
        maxHeight: '90vh',
        panelClass: 'custom-dialog-container',
        data: {
          selectedMerken: this.shopParams.merken,
          selectedTypes: this.shopParams.types
        }
      });
  
      //na het sluiten van de dialog, de gekozen merken en types (result.) toevoegen aan de string arrays selectedMerken en selectedTypes
      dialogRef.afterClosed().subscribe({
        next: result => {
          if (result){
            this.shopParams.merken = result.selectedMerken;
            this.shopParams.types = result.selectedTypes;
            this.shopParams.pageNumber = 1;
            this.loadProducts();
          }
        }
      });
    }
  
    resetFilters() {
      this.shopParams = new ShopParams();
      this.loadProducts();
    }
}
