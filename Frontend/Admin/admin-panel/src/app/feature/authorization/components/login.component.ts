import { Component, computed, signal, ChangeDetectionStrategy, inject } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { SHARED_IMPORTS } from '../../../../shared-module';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  imports: [...SHARED_IMPORTS],
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthorizationService);

  loginForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  errorMessage = signal('');

  isSubmitting = signal(false);

  private formStatus = toSignal(this.loginForm.statusChanges, { initialValue: this.loginForm.status });

  isFormInvalid = computed(() => this.formStatus() !== 'VALID' || this.isSubmitting());

  login(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const { email, password } = this.loginForm.getRawValue();

    this.authService.login(email, password).subscribe({
      next: () => {
        this.router.navigate(['/roles']);
      },
      error: () => {
        this.errorMessage.set('Invalid email or password');
        this.isSubmitting.set(false);
      },
    });
  }
}