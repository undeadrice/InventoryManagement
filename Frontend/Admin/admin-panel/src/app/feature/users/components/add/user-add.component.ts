import { Component, ChangeDetectionStrategy, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { RoleService } from '../../../roles/services/role.service';
import { RoleSimpleResponse } from '../../../roles/models/responses/role-simple.response';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';

@Component({
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
  ],
  templateUrl: './user-add.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserAddComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  readonly form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    dateOfBirth: ['', [Validators.required]],
    roleIds: [[] as string[]],
  });

  readonly roles = signal<RoleSimpleResponse[]>([]);
  readonly loading = signal(true);
  readonly submitting = signal(false);

  ngOnInit(): void {
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.roles.set(roles);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load roles', 'Close', {
          duration: 5000,
        });
        this.loading.set(false);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    const raw = this.form.value;
    const dateOfBirth =
      raw.dateOfBirth instanceof Date
        ? raw.dateOfBirth.toISOString().split('T')[0]
        : raw.dateOfBirth;

    this.submitting.set(true);
    this.userService
      .createUser({
        firstName: raw.firstName,
        lastName: raw.lastName,
        email: raw.email,
        password: raw.password,
        dateOfBirth,
        roleIds: raw.roleIds,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('User created successfully', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/users']);
        },
        error: () => {
          this.submitting.set(false);
        },
      });
  }
}