import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDivider } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product';
import { MatDialog } from '@angular/material/dialog';
import { ProductSuccessDialogComponent } from '../product-success-dialog/product-success-dialog.component';
import { ShopService } from '../../../core/services/shop.service';

@Component({
  selector: 'app-update-product',
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
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.scss'
})
export class UpdateProductComponent implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private dialogService = inject(MatDialog);
  private activatedRoute = inject(ActivatedRoute);
  private shopService = inject(ShopService);
  product?: Product;
  validationErrors?: string[];
  updatedProduct: Product | undefined;

  productForm = this.fb.group<{
    id: FormControl<number>;
    naam: FormControl<string>;
    prijs: FormControl<number>;
    merk: FormControl<string>;
    type: FormControl<string>;
    image: FormControl<File | null>;
    hoeveelheidInVoorraad: FormControl<number>;
    beschrijving: FormControl<string>;
    fotoUrl: FormControl<string>;
  }>({
    id: this.fb.control(0, {nonNullable: true, validators: Validators.required }),
    naam: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    prijs: this.fb.control(0.00, { nonNullable: true, validators: Validators.required }),
    merk: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    type: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    image: this.fb.control(null),
    hoeveelheidInVoorraad: this.fb.control(0, { nonNullable: true, validators: Validators.required }),
    beschrijving: this.fb.control('', { nonNullable: true, validators: Validators.required }),
    fotoUrl: this.fb.control('', {nonNullable: true, validators: Validators.required })
  });

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct(){
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if(!id) return;

    this.shopService.getProduct(+id).subscribe({
      next: product => {
        if (product) {
          this.productForm.patchValue({
            id: product.id,
            naam: product.naam,
            prijs: product.prijs,
            merk: product.merk,
            type: product.type,
            hoeveelheidInVoorraad: product.hoeveelheidInVoorraad,
            beschrijving: product.beschrijving,
            fotoUrl: product.fotoURL
          });
          this.product = product;
        }
      },
      error: error => console.log(error)
    });
  }

  onSubmit() {
    if (this.productForm.valid) {
      const formData = new FormData();

      const values = this.productForm.value;
      if(values.id) {
        formData.append('id', values.id.toString());
      }
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

      formData.append('fotoUrl', values.fotoUrl ?? '');

      this.productService.updateProduct(formData, this.product?.id || 0).subscribe({
        next: product => {
          this.updatedProduct = product;
          this.openSuccessDialog();
        },
        error: errors => this.validationErrors = errors
      })
   }
  }

  onSelectedFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.productForm.patchValue({ image: file });
    }
  }

  openSuccessDialog() {
    const minWidth = window.innerWidth < 640 ? '90vw' : '500px';
    this.dialogService.open(ProductSuccessDialogComponent, {
      minWidth: minWidth,
      maxWidth: '95vw',
      maxHeight: '90vh',
      panelClass: 'custom-dialog-container',
      data: {
        product: this.updatedProduct,
        titel: 'U heeft volgend product aangepast.'
      }
    });
  }
}
