import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { DashboardLayoutComponent } from './layout/dashboard-layout/dashboard-layout.component';

const routes: Routes = [
  // Default: redirect root URL to /login
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  // Public route — lazy load the AuthModule only when the user visits /login
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },

  // Protected route — the AuthGuard blocks access if the user is not logged in.
  // DashboardLayoutComponent is the persistent shell (sidebar + toolbar).
  // Epic 5 will add children here for Categories and Products.
  {
    path: 'dashboard',
    component: DashboardLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'categories', pathMatch: 'full' },
      { 
        path: 'categories', 
        loadChildren: () => import('./features/categories/categories.module').then(m => m.CategoriesModule) 
      },
      { 
        path: 'products', 
        loadChildren: () => import('./features/products/products.module').then(m => m.ProductsModule) 
      },
      { 
        path: 'ai-chat', 
        loadChildren: () => import('./features/ai-chat/ai-chat.module').then(m => m.AiChatModule) 
      }
    ]
  },

  // Catch-all: any unknown URL redirects to login
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
