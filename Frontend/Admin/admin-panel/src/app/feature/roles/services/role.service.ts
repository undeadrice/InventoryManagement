import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseHttpService } from '../../../core/http/base-http.service';
import { RoleSimpleResponse } from '../models/responses/role-simple.response';
import { RoleResponse } from '../models/responses/role.response';
import { PermissionGroupResponse } from '../models/responses/permission-group.response';
import { CreateRoleRequest } from '../models/requests/create-role.request';
import { UpdateRoleRequest } from '../models/requests/update-role.request';

@Injectable({
  providedIn: 'root',
})
export class RoleService extends BaseHttpService {

  getRoles(): Observable<RoleSimpleResponse[]> {
    return this.get<RoleSimpleResponse[]>('roles');
  }

  getRole(id: string): Observable<RoleResponse> {
    return this.get<RoleResponse>(`roles/${id}`);
  }

  getPermissions(): Observable<PermissionGroupResponse[]> {
    return this.get<PermissionGroupResponse[]>('roles/permissions');
  }

  createRole(request: CreateRoleRequest): Observable<string> {
    return this.post<string>('roles', request);
  }

  updateRole(request: UpdateRoleRequest): Observable<void> {
    return this.put<void>('roles/update', request);
  }
}
