export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  dOB: string;
  gender: string;
  address?: string | null;
}
