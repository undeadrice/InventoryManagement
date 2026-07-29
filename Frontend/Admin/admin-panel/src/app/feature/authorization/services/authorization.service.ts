import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { LoginResponse } from '../models/login-response.model';
import { BaseHttpService } from '../../../core/http/base-http.service';
import { AuthService } from '../../../core/auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationService extends BaseHttpService {
  private readonly authService = inject(AuthService);

  login(email: string, password: string): Observable<LoginResponse> {
    return this.post<LoginResponse>('auth/login', { email, password }).pipe(
      tap((response) => {
        this.authService.setToken(response.token);
      }),
    );
  }
}