import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'register',
        loadComponent: () => import('./features/auth/pages/register-page/register-page.component').then(m => m.RegisterPageComponent)
      },
      {
        path: 'verify-email',
        loadComponent: () => import('./features/auth/pages/verify-email-page/verify-email-page.component').then(m => m.VerifyEmailPageComponent)
      },
      { path: '**', redirectTo: 'register', pathMatch: 'full' }
    ]
  },
  {
    path: 'public',
    children: [
      {
        path: 'home',
        loadComponent: () => import('./features/public/pages/home-page/home-page.component').then(m => m.HomePageComponent)
      },
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ] 
  },
  { path: '**', redirectTo: 'public', pathMatch: 'full' }
];
