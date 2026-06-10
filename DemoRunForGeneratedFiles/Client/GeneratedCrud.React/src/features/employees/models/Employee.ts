export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  dob: string;
  gender: string;
  address?: string | null;
  createdDate: string;
  updatedDate?: string | null;
  isDeleted: boolean;
  departmentId?: number | null;
  designationId?: number | null;
  departmentName?: string | null;
  designationName?: string | null;
}
