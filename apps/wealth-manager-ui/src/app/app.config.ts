import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { appRoutes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor, errorInterceptor } from '@wealth-manager/shared/data-access';
import { provideAnimations } from '@angular/platform-browser/animations';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(appRoutes),
  provideHttpClient(withFetch(), withInterceptors([authInterceptor, errorInterceptor])),
  provideAnimations()
  ],
};
