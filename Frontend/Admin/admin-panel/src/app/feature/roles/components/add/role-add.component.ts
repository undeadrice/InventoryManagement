import { Component, ChangeDetectionStrategy, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { RoleService } from '../../services/role.service';
import { PermissionGroupResponse } from '../../models/responses/permission-group.response';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

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
    MatCheckboxModule,
    MatProgressBarModule,
  ],
  templateUrl: './role-add.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleAddComponent implements OnInit {
  private fb = inject(FormBuilder);
  private roleService = inject(RoleService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  readonly form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
  });

  readonly permissionGroups = signal<PermissionGroupResponse[]>([]);
  readonly loading = signal(true);
  readonly submitting = signal(false);

  ngOnInit(): void {
    this.roleService.getPermissions().subscribe({
      next: (groups) => {
        const permissionsFormGroup: Record<string, boolean> = {};
        for (const group of groups) {
          for (const perm of group.permissions) {
            permissionsFormGroup[perm] = false;
          }
        }

        this.form.addControl(
          'permissions',
          this.fb.group(permissionsFormGroup),
        );

        this.permissionGroups.set(groups);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load permissions', 'Close', {
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

    const rawPermissions = this.form.get('permissions')!.value as Record<
      string,
      boolean
    >;
    const selectedPermissions = Object.keys(rawPermissions).filter(
      (key) => rawPermissions[key],
    );

    this.submitting.set(true);
    this.roleService
      .createRole({
        name: this.form.get('name')!.value,
        permissions: selectedPermissions,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Role created successfully', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/roles']);
        },
        error: () => {
          this.snackBar.open('Failed to create role', 'Close', {
            duration: 5000,
          });
          this.submitting.set(false);
        },
      });
  }
}