import { Routes } from '@angular/router';
import { AuthorizedLayoutComponent } from './shared/authorized/authorized-layout.component';
import { UnauthorizedLayoutComponent } from './shared/unauthorized/unauthorized-layout.component';
import { RoleListcomponent } from './feature/roles/components/list/role-list.component';
import { UserListComponent } from './feature/users/components/user-list/user-list.component';
import { LoginComponent } from './feature/authorization/components/login.component';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: AuthorizedLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'roles',
        component: RoleListcomponent,
      },
      {
        path: 'users',
        component: UserListComponent,
      },
      {
        path: '',
        redirectTo: 'roles',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '',
    component: UnauthorizedLayoutComponent,
    children: [
      {
        path: 'login',
        component: LoginComponent,
      },
    ],
  },
];
