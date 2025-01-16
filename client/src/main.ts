import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { DEFAULT_CURRENCY_CODE } from '@angular/core';

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []), // Voeg de bestaande providers toe
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'EUR' } // Voeg currency code euro toe
  ]
})
  .catch((err) => console.error(err));
