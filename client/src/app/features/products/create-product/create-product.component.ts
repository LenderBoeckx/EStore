import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDivider } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RouterLink } from '@angular/router';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { MatDialog } from '@angular/material/dialog';
import { ProductSuccessDialogComponent } from '../product-success-dialog/product-success-dialog.component';

@Component({
  selector: 'app-create-product',
  standalone: true,
  imports: [
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIcon,
    RouterLink,
    MatDivider,
    TextInputComponent,
    ReactiveFormsModule,
    MatInput,
    MatCard
],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.scss'
})
export class CreateProductComponent {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private dialogService = inject(MatDialog);
  validationErrors?: string[];
  createdProduct: Product | undefined;

  productForm = this.fb.group<{
    naam: FormControl<string>;
    prijs: FormControl<number>;
    merk: FormControl<string>;
    type: FormControl<string>;
    image: FormControl<File | null>;
    hoeveelheidInVoorraad: FormControl<number>;
    beschrijving: FormControl<string>;
  }>({
    naam: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    prijs: this.fb.control(0.00, { nonNullable: true, validators: Validators.required }),
    merk: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    type: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    image: this.fb.control(null, { validators: Validators.required }),
    hoeveelheidInVoorraad: this.fb.control(0, { nonNullable: true, validators: Validators.required }),
    beschrijving: this.fb.control('', { nonNullable: true, validators: Validators.required }),
  });

  onSubmit() {
    if (this.productForm.valid) {
      const formData = new FormData();

      const values = this.productForm.value;

      formData.append('naam', values.naam ?? '');
      if(values.prijs){
        formData.append('prijs', values.prijs?.toString());
      }
      formData.append('merk', values.merk ?? '');
      formData.append('type', values.type ?? '');
      if(values.hoeveelheidInVoorraad) {
        formData.append('hoeveelheidInVoorraad', values.hoeveelheidInVoorraad.toString() ?? '0');
      } 
      formData.append('beschrijving', values.beschrijving ?? '');

      if(values.image) {
        formData.append('image', values.image);
      }

      this.productService.createProduct(formData).subscribe({
        next: newProduct => {
          this.createdProduct = newProduct;
          this.openSuccessDialog();
        },
        error: errors => this.validationErrors = errors
      })
   }
  }

  openSuccessDialog() {
    const dialogRef = this.dialogService.open(ProductSuccessDialogComponent, {
      minWidth: '500px',
      data: {
        product: this.createdProduct
      }
    });
  }

  onSelectedFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;

  if (input.files && input.files.length > 0) {
    const file = input.files[0];
    this.productForm.patchValue({ image: file });
  }
  }

}
