import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
    const payload = JSON.parse(atob(token.split('.')[1]));
    const validTo = new Date(payload.exp * 1000);
    localStorage.setItem('token-valid-to', validTo.toUTCString());
  }
}