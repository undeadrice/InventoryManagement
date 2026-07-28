import { Component, ChangeDetectionStrategy } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { SHARED_IMPORTS } from '../../../../../shared-module';
import { UserSimpleResponse } from '../../models/responses/user-simple.response';

@Component({
  imports: [...SHARED_IMPORTS],
  templateUrl: './user-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserListComponent {
  users = new MatTableDataSource<UserSimpleResponse>([]);
  displayedColumns: string[] = ['Email', 'Firstname', 'Lastname', 'actions'];

  constructor(
    private userService: UserService,
    private router: Router,
  ) {
    this.userService.getUsers().subscribe((data) => {
      this.users.data = data;
    });
  }

  editUser(id: string): void {
    this.router.navigate(['users/edit', id]);
  }
}