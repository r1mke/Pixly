import { Routes } from '@angular/router';


export const routes: Routes = [
    {
        path: 'public',
        loadComponent: () => import('./features/public/public/public.component').then(m => m.PublicComponent)
      },
      {
        path: 'auth',
        loadComponent: () => import('./features/auth/auth/auth.component').then(m => m.AuthComponent)
      },
      {
        path: 'admin',
        loadComponent: () => import('./features/admin/admin/admin.component').then(m => m.AdminComponent)  
      },
      {path: '**', redirectTo: 'public', pathMatch: 'full'}
];
