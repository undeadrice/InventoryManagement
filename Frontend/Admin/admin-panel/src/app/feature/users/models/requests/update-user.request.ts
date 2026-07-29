export interface UpdateUserRequest {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  email: string;
  roleIds: string[];
}