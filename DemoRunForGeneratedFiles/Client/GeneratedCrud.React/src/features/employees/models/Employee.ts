export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  dOB: string;
  gender: string;
  address?: string | null;
  createdDate: string;
  updatedDate?: string | null;
  isDeleted: boolean;
}
