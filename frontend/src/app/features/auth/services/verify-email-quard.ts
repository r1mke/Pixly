import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VerifyEmailGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(): Observable<boolean> {
    // Check if running in browser
    if (typeof window !== 'undefined' && window.localStorage) {
      const token = localStorage.getItem('jwtToken');
      
      if (token) {
        return of(true);  // Token exists, allow access
      } else {
        this.router.navigate(['/login']);
        return of(false);  // No token, deny access
      }
    } else {
      // Fallback for SSR or environments where localStorage is not available
      this.router.navigate(['/login']);
      return of(false);
    }
  }
}
