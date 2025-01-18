import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { JsonPipe } from '@angular/common';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatLabel,
    MatInput,
    MatButton,
    JsonPipe,
    TextInputComponent
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private snack = inject(SnackbarService);
  validationErrors?: string[];

  //nieuw reactive form aanmaken met de verschillende user properties die nodig zijn om een nieuwe gebruiker aan te maken in de database
  //validators toevoegen voor de user interface
  //bijkomende password validators worden gecontroleerd op de server, (pattern) kan nog toegevoegd worden om een dubbele controle uit te voeren, zowel client als server side
  registerForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  //acties wanneer de user op de knop registreren klikt
  //de account service aanspreken om een nieuwe user aan te maken in de database
  //bij succes een snack tonen onderaan het scherm met een succesvolle boodschap
  //bij een fout, de errors toevoegen aan de validation errors lijst
  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.snack.success('Registratie succesvol - U kan nu inloggen.');
        this.router.navigateByUrl('/account/login');
      },
      error: errors => this.validationErrors = errors
    });
  }

}
