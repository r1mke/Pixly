/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideAnimations } from '@angular/platform-browser/animations';
bootstrapApplication(AppComponent, {
  ...appConfig, // Uključi tvoju konfiguraciju aplikacije
  providers: [
    provideAnimations(), // Angular funkcija za animacije
    ...(appConfig.providers || []) // Dodaj providere iz appConfig-a ako ih ima
  ],
}).catch((err) => console.error(err));
