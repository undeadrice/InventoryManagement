import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseHttpService } from '../../../core/http/base-http.service';
import { UserSimpleResponse } from '../models/responses/user-simple.response';
import { UserResponse } from '../models/responses/user.response';
import { CreateUserRequest } from '../models/requests/create-user.request';
import { UpdateUserRequest } from '../models/requests/update-user.request';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseHttpService {

  getUsers(): Observable<UserSimpleResponse[]> {
    return this.get<UserSimpleResponse[]>('users');
  }

  getUser(id: string): Observable<UserResponse> {
    return this.get<UserResponse>(`users/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<void> {
    return this.post<void>('users', request);
  }

  updateUser(request: UpdateUserRequest): Observable<void> {
    return this.put<void>('users/update', request);
  }
}
