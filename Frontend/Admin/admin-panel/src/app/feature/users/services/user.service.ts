import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseHttpService } from '../../../core/http/base-http.service';
import { AuthService } from '../../../core/auth/auth.service';
import { UserSimpleResponse } from '../models/responses/user-simple.response';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseHttpService {
  constructor(authService: AuthService, httpClient: HttpClient) {
    super(httpClient, authService);
  }

  getUsers(): Observable<UserSimpleResponse[]> {
    return this.get<UserSimpleResponse[]>('users');
  }
}