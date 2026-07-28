import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('token-valid-to');
        router.navigate(['/login']);
        snackBar.open('Session expired. Please log in again.', 'Close', {
          duration: 5000,
          panelClass: 'snackbar-error',
        });
        return throwError(() => error);
      }

      const detail = extractErrorMessage(error);
      snackBar.open(detail, 'Close', {
        duration: 5000,
        panelClass: 'snackbar-error',
      });

      return throwError(() => error);
    }),
  );
};

function extractErrorMessage(error: HttpErrorResponse): string {
  if (typeof error.error === 'string') {
    return error.error;
  }

  if (error.error?.detail) {
    return error.error.detail;
  }

  if (error.error?.title) {
    return error.error.title;
  }

  if (error.message) {
    return error.message;
  }

  return 'An unexpected error occurred.';
}