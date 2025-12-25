import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            const isLoginRequest = req.url.includes('/login');
            if (error.status === 401) {
                if (isLoginRequest) {
                    return throwError(() => error);
                }

                console.error('Unauthorized! Clear session & Redirect to login');
                localStorage.clear();
                router.navigate(['/login']);
            } else if (error.status === 403) {
                console.error('Forbidden! You dont have permission.');
            }

            if (error.error?.errorId) {
                console.warn(`API Error ID: ${error.error.errorId}`);
            }

            return throwError(() => error);
        })
    )
}