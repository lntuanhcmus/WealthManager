import { Route } from '@angular/router';
import { LoginComponent } from '@wealth-manager/features/auth';
import { authGuard } from '@wealth-manager/shared/data-access';

export const appRoutes: Route[] = [
    { path: 'login', component: LoginComponent },
    {
        path: '',
        canActivate: [authGuard],
        children: []
    },

    { path: '**', redirectTo: 'login' }
];
