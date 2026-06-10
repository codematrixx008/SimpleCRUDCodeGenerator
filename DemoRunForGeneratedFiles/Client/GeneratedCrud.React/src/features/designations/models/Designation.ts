export interface Designation {
  id: number;
  designationName: string;
  designationCode: string;
  description?: string | null;
  createdDate: string;
  updatedDate?: string | null;
  isDeleted: boolean;
}
