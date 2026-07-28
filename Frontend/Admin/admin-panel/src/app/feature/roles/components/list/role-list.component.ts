import { Component, ChangeDetectionStrategy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { RoleService } from '../../services/role.service';
import { SHARED_IMPORTS } from '../../../../../shared-module';
import { RoleSimpleResponse } from '../../models/responses/role-simple.response';

@Component({
  imports: [...SHARED_IMPORTS],
  templateUrl: './role-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleListcomponent {
  roles = new MatTableDataSource<RoleSimpleResponse>([]);
  displayedColumns: string[] = ['Name', 'actions'];

  constructor(
    private roleService: RoleService,
    private router: Router,
  ) {
    this.roleService.getRoles().subscribe((data) => {
      this.roles.data = data;
    });
  }

  edit(id: string): void {
    this.router.navigate(['roles/edit', id]);
  }
}