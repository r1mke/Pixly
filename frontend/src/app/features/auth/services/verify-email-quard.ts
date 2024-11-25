import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthService } from './auth.service';
import { map, catchError, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class VerifyEmailGuard implements CanActivate {
  constructor(private router: Router, private authService: AuthService) {}

  canActivate(): Observable<boolean> {
    return this.authService.getCurrentUser().pipe(
      map((response: any) => {
        if (!response?.isValid) {
          this.router.navigate(['/auth/login']);
          return false;
        }
        if (response.user?.isVerified) {
          this.router.navigate(['/public/home']);
          return false; 
        }
        return true;
      }),
      catchError((error) => {
        console.error('Error in VerifyEmailGuard:', error);
        this.router.navigate(['/auth/login']);
        return of(false);
      })
    );
  }
}
