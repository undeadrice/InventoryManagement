import { Injectable, computed, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly firstName = signal<string>('');
  private readonly lastName = signal<string>('');

  readonly fullName = computed(() => {
    const first = this.firstName();
    const last = this.lastName();
    if (first && last) {
      return `${first} ${last}`;
    }
    return first || last || '';
  });

  constructor() {
    this.loadUserFromToken();
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
    const payload = JSON.parse(atob(token.split('.')[1]));
    const validTo = new Date(payload.exp * 1000);
    localStorage.setItem('token-valid-to', validTo.toUTCString());

    this.decodeUserFromPayload(payload);
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (!token) {
      return;
    }

    const payload = JSON.parse(atob(token.split('.')[1]));
    this.decodeUserFromPayload(payload);
  }

  private decodeUserFromPayload(payload: Record<string, unknown>): void {
    const firstName = (payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']
      ?? payload['given_name']
      ?? '') as string;
    const lastName = (payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']
      ?? payload['family_name']
      ?? '') as string;

    this.firstName.set(firstName);
    this.lastName.set(lastName);
  }
}
