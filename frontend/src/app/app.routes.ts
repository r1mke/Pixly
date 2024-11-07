import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'register',
        loadComponent: () => import('./features/auth/pages/register-page/register-page.component').then(m => m.RegisterPageComponent)
      },
      { path: '**', redirectTo: 'register', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'public', pathMatch: 'full' }
];
