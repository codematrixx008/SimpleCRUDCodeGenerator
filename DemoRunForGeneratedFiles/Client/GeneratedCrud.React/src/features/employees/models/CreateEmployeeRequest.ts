export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  dob: string;
  gender: string;
  address?: string | null;
  departmentId?: number | null;
  designationId?: number | null;
}
