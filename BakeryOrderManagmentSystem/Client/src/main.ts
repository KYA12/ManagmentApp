import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { environment } from './environments/environments.development';

// Check if the environment is production and enable production mode if true
if (environment.production) {
  enableProdMode();
}

// Bootstrap the root module of the application
platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));