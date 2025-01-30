import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { Product } from '../../../shared/models/product';
import { Router } from '@angular/router';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product-success-dialog',
  standalone: true,
  imports: [
    MatDividerModule,
        MatSelectModule,
        MatListModule,
        MatButton,
        FormsModule,
        MatDividerModule,
        CurrencyPipe
  ],
  templateUrl: './product-success-dialog.component.html',
  styleUrl: './product-success-dialog.component.scss'
})
export class ProductSuccessDialogComponent {
  data = inject(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<ProductSuccessDialogComponent>);
  product: Product = this.data.product;
  private router = inject(Router);
  titel: string = this.data.titel;
  

  closeDialog() {
    this.dialogRef.close();
    this.router.navigateByUrl('/admin');
  }
}
