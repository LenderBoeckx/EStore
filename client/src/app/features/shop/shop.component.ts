import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { MatCardModule } from '@angular/material/card';
import { ProductItemComponent } from "./product-item/product-item.component";
import {MatDialog, MatDialogModule} from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { ShopParams } from '../../shared/models/shopParams';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
import { EmptyStateComponent } from "../../shared/components/empty-state/empty-state.component";

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [MatCardModule,
    ProductItemComponent,
    MatDialogModule,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectModule,
    MatListModule,
    MatMenuTrigger,
    MatPaginatorModule,
    FormsModule, EmptyStateComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products?: Pagination<Product>;
  sortOptions = [
    {naam: 'Alphabetisch', value: 'naam'},
    {naam: 'Prijs: Laag-Hoog', value: 'prijsAsc'},
    {naam: 'Prijs: Hoog-Laag', value: 'prijsDesc'}
  ]
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];

  //functies oproepen bij het initialiseren van de component
  ngOnInit(): void {
    this.initializeShop();
  }

  //opdracht geven om de merken, types en gewenste aantal producten op te halen vanuit de database
  initializeShop() {
    this.shopService.getMerken();
    this.shopService.getTypes();
    this.getProducts();
  }

  //producten ophalen uit de database voor de gewenste pagina en aantal items per pagina
  handlePageEvent(event: PageEvent){
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();
  }

  //producten ophalen wanneer de sorteringskeuze gewijzigd is
  onSortChange(event: MatSelectionListChange){
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sort = selectedOption.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
  }

  //producten ophalen wanneer de zoekterm gewijzigd is
  onSearchChange(){
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }
  
  //producten ophalen vanuit de database
  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => this.products = response,
      error: error => console.log(error)
    });
  }

  //functie getriggerd door een klik event op de knop filters
  openFiltersDialog(){
    const minWidth = window.innerWidth < 640 ? '90vw' : '500px';
    //dialog van material openen met de opties om merken en/of types te filteren
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
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
          this.getProducts();
        }
      }
    });
  }

  resetFilters() {
    this.shopParams = new ShopParams();
    this.getProducts();
  }
}
