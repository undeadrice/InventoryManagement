import { Component, ChangeDetectionStrategy, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';

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
  providers: [provideNativeDateAdapter()],
  templateUrl: './user-edit.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    dateOfBirth: ['', [Validators.required]],
    roleIds: [[] as string[]],
  });

  readonly roles = signal<RoleSimpleResponse[]>([]);
  readonly loading = signal(true);
  readonly submitting = signal(false);

  private userId: string | null = null;

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id');

    if (!this.userId) {
      this.snackBar.open('Invalid user ID', 'Close', { duration: 5000 });
      this.router.navigate(['/users']);
      return;
    }

    forkJoin({
      user: this.userService.getUser(this.userId),
      roles: this.roleService.getRoles(),
    }).subscribe({
      next: ({ user, roles }) => {
        const parsedDate =
          typeof user.dateOfBirth === 'string'
            ? new Date(user.dateOfBirth)
            : user.dateOfBirth;

        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          dateOfBirth: parsedDate,
          roleIds: user.roleIds,
        });

        this.roles.set(roles);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load user', 'Close', {
          duration: 5000,
        });
        this.router.navigate(['/users']);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid || !this.userId) {
      return;
    }

    const raw = this.form.value;
    const dateOfBirth =
      raw.dateOfBirth instanceof Date
        ? raw.dateOfBirth.toISOString().split('T')[0]
        : raw.dateOfBirth;

    this.submitting.set(true);
    this.userService
      .updateUser({
        id: this.userId,
        firstName: raw.firstName,
        lastName: raw.lastName,
        email: raw.email,
        dateOfBirth,
        roleIds: raw.roleIds,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('User updated successfully', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/users']);
        },
        error: () => {
          this.snackBar.open('Failed to update user', 'Close', {
            duration: 5000,
          });
          this.submitting.set(false);
        },
      });
  }
}