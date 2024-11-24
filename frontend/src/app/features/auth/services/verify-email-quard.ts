import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { map, Observable, of, switchMap } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class VerifyEmailGuard implements CanActivate {
  constructor(private router: Router, private authService: AuthService) {}

  canActivate(): Observable<boolean> {
    return this.authService.verifyJwtToken().pipe(
      switchMap((response: any) => {
        if (response.isValid) {
          return this.authService.getCurrentUser().pipe(
            map((user: any) => {
              if (user?.isVerified) {
                this.router.navigate(['/public/home']);
                return false;
              } else {
                return true;
              }
            })
          );
        } else {
          this.router.navigate(['/public/home']);
          return of(false);
        }
      })
    );
  }
}
