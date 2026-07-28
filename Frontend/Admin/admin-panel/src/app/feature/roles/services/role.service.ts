import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseHttpService } from '../../../core/http/base-http.service';
import { AuthService } from '../../../core/auth/auth.service';
import { RoleSimpleResponse } from '../models/responses/role-simple.response';

@Injectable({
  providedIn: 'root',
})
export class RoleService extends BaseHttpService {
  constructor(authService: AuthService, httpClient: HttpClient) {
    super(httpClient, authService);
  }

  getRoles(): Observable<RoleSimpleResponse[]> {
    return this.get<RoleSimpleResponse[]>('roles');
  }
}