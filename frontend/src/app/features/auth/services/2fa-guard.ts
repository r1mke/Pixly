import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service'; // Pretpostavimo da imaš AuthService
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TwoFaGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean | Observable<boolean> {
    const statusCode = sessionStorage.getItem('statusCode');

    if (statusCode === '202')
      return true;
    
    this.router.navigate(['/login']);
    return false;
  }
}
