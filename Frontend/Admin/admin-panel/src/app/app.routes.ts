import { Routes } from '@angular/router';
import { AuthorizedLayoutComponent } from './shared/authorized/authorized-layout.component';
import { UnauthorizedLayoutComponent } from './shared/unauthorized/unauthorized-layout.component';
import { RoleListcomponent } from './feature/roles/components/list/role-list.component';
import { RoleAddComponent } from './feature/roles/components/add/role-add.component';
import { RoleEditComponent } from './feature/roles/components/edit/role-edit.component';
import { UserListComponent } from './feature/users/components/user-list/user-list.component';
import { UserAddComponent } from './feature/users/components/add/user-add.component';
import { UserEditComponent } from './feature/users/components/edit/user-edit.component';
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
        path: 'roles/add',
        component: RoleAddComponent,
      },
      {
        path: 'roles/edit/:id',
        component: RoleEditComponent,
      },
      {
        path: 'users',
        component: UserListComponent,
      },
      {
        path: 'users/add',
        component: UserAddComponent,
      },
      {
        path: 'users/edit/:id',
        component: UserEditComponent,
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
