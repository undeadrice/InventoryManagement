export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  email: string;
  password: string;
  roleIds: string[];
}