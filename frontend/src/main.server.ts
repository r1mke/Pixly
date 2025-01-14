import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';
import { provideClientHydration } from '@angular/platform-browser';

const updatedConfig = {
    ...config,
    providers: [
      ...(config.providers || []), // Zadrži postojeće provajdere
      provideClientHydration()    // Dodaj hydration
    ]
  };

const bootstrap = () => bootstrapApplication(AppComponent, updatedConfig);

export default bootstrap;
