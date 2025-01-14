import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { map, catchError, of } from 'rxjs';

export const adminGuard: CanActivateFn = (childRoute, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.getCurrentUser().pipe(
    map((response: any) => {
      if (!response?.isValid || !response.user.isAdmin) {
        router.navigate(['/auth/login']);
        return false;
      }
      return true;
    }),
    catchError((error) => {
      console.error('Error in AuthGuard:', error);
      router.navigate(['/auth/login']);
      return of(false);
    })
  );
};
