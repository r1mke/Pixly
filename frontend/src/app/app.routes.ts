import { Routes } from '@angular/router';
import { VerifyEmailGuard } from './features/auth/services/verify-email-quard';
import { AuthGuard } from './features/auth/services/auth-guard';

export const routes: Routes = [
  {
    path: 'admin',
    children: [
     {
       path: 'dashboard',
       loadComponent: () => import('./features/admin/pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
     },
     {
       path: 'new-posts',
       loadComponent: () => import('./features/admin/pages/new-posts/new-posts.component').then(m => m.NewPostsComponent),
     },
     {
      path: 'users',
      loadComponent: () => import('./features/admin/pages/users/users.component').then(m => m.UsersComponent),
     },
     {
      path: 'user/:username',
      loadComponent: () => import('./features/admin/pages/user-profile/user-profile.component').then(m => m.UserProfileComponent),
     } 
    ]
  },
  {
    path: 'auth',
    children: [
      {
        path: 'register',
        loadComponent: () => import('./features/auth/pages/register-page/register-page.component').then(m => m.RegisterPageComponent)
      },
      {
        path: 'verify-email',
        loadComponent: () => import('./features/auth/pages/verify-email-page/verify-email-page.component').then(m => m.VerifyEmailPageComponent),
        canActivate: [VerifyEmailGuard]
      },
      {
        path: 'login',
        loadComponent: () => import('./features/auth/pages/login-page/login-page.component').then(m => m.LoginPageComponent),
      },
      {
        path: 'edit-profile',
        loadComponent: () => import('./features/auth/pages/edit-profile-page/edit-profile-page.component').then(m => m.EditProfilePageComponent),
        canActivate: [AuthGuard]
      },
      {
        path: 'photo/:id/edit',
        loadComponent: () => import('./features/auth/pages/edit-photo-page/edit-photo-page.component').then(m => m.EditPhotoPageComponent),
        canActivate: [AuthGuard]
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
      {
        path: 'profile',
        children: [
          {
            path: 'user/:username',
            loadComponent: () => import('./features/public/pages/profile-page/profile-page.component').then(m => m.ProfilePageComponent),
          },
          {
            path: 'user/:username/liked',
            loadComponent: () => import('./features/public/pages/profile-page/profile-page.component').then(m => m.ProfilePageComponent)
            
          },
          {
            path: 'user/:username/gallery',
            loadComponent: () => import('./features/public/pages/profile-page/profile-page.component').then(m => m.ProfilePageComponent)
          },
         
          {
            path: 'upload',
            loadComponent: () => import('./features/public/pages/upload-page/upload-page.component').then(m => m.UploadPageComponent),
            canActivate: [AuthGuard]
          }
        ]
        /* {
        path: 'profile/user/:username',
        loadComponent: () => import('./features/public/pages/profile-page/profile-page.component').then(m => m.ProfilePageComponent),
        children: [
          { path: '', redirectTo: 'gallery', pathMatch: 'full' },
          { path: 'gallery', component: GalleryComponent },
          { path: 'liked', component: GalleryComponent },
          { path: 'collections', component: GalleryComponent },
          { path: 'ai', component: GalleryComponent }
        ]
        },*/
      },
      {
        path: 'generate-image',
        loadComponent: () => import('./features/public/pages/generate-image-page/generate-image-page.component').then(m => m.GenerateImagePageComponent),
      },
      {
        path: 'photo/:id',
        loadComponent: () => import('./features/public/pages/photo-page/photo-page.component').then(m => m.PhotoPageComponent)
      },
      {
        path: 'search',
        loadComponent: () => import('./features/public/pages/search-page/search-page.component').then(m => m.SearchPageComponent)
      },
      { path: '**', redirectTo: 'home', pathMatch: 'full' }
    ] 
  },
  { path: '**', redirectTo: 'public', pathMatch: 'full' }
];
