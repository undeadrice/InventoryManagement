import { Component, ChangeDetectionStrategy, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
    MatCheckboxModule,
    MatProgressBarModule,
  ],
  templateUrl: './role-edit.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleEditComponent implements OnInit {
  private fb = inject(FormBuilder);
  private roleService = inject(RoleService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
  });

  readonly permissionGroups = signal<PermissionGroupResponse[]>([]);
  readonly loading = signal(true);
  readonly submitting = signal(false);

  private roleId: string | null = null;

  ngOnInit(): void {
    this.roleId = this.route.snapshot.paramMap.get('id');

    if (!this.roleId) {
      this.snackBar.open('Invalid role ID', 'Close', { duration: 5000 });
      this.router.navigate(['/roles']);
      return;
    }

    forkJoin({
      role: this.roleService.getRole(this.roleId),
      permissions: this.roleService.getPermissions(),
    }).subscribe({
      next: ({ role, permissions }) => {
        this.form.patchValue({ name: role.name });

        const permissionsFormGroup: Record<string, boolean> = {};
        for (const group of permissions) {
          for (const perm of group.permissions) {
            permissionsFormGroup[perm] = role.permissions.includes(perm);
          }
        }

        this.form.addControl(
          'permissions',
          this.fb.group(permissionsFormGroup),
        );

        this.permissionGroups.set(permissions);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load role', 'Close', {
          duration: 5000,
        });
        this.router.navigate(['/roles']);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid || !this.roleId) {
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
      .updateRole({
        id: this.roleId,
        name: this.form.get('name')!.value,
        permissions: selectedPermissions,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Role updated successfully', 'Close', {
            duration: 3000,
          });
          this.router.navigate(['/roles']);
        },
        error: () => {
          this.snackBar.open('Failed to update role', 'Close', {
            duration: 5000,
          });
          this.submitting.set(false);
        },
      });
  }
}