import { Component, inject } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-filters-dialog',
  standalone: true,
  imports: [
    MatDividerModule,
    MatSelectModule,
    MatListModule,
    MatButton,
    FormsModule
  ],
  templateUrl: './filters-dialog.component.html',
  styleUrl: './filters-dialog.component.scss'
})
export class FiltersDialogComponent {
  shopService = inject(ShopService);
  private dialogRef = inject(MatDialogRef<FiltersDialogComponent>);
  data = inject(MAT_DIALOG_DATA);

  selectedMerken: string[] = this.data.selectedMerken;
  selectedTypes: string[] = this.data.selectedTypes;

  //functie die getriggerd wordt bij het klikken op de knop filters toepassen, hierna sluit ook de dialog en slaat deze de gekozen gegevens
  applyFilters() {
    this.dialogRef.close({
      selectedMerken: this.selectedMerken,
      selectedTypes: this.selectedTypes
    });
  }

  //functie die getriggerd wordt bij het klikken op de knop filters verwijderen, hierna sluit de dialog en worden de filters gereset
  removeFilters() {
    this.dialogRef.close({
      selectedMerken: [],
      selectedTypes: []
    });
  }
}
