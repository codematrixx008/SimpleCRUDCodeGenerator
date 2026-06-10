export interface Department {
  id: number;
  departmentName: string;
  departmentCode: string;
  description?: string | null;
  createdDate: string;
  updatedDate?: string | null;
  isDeleted: boolean;
}
