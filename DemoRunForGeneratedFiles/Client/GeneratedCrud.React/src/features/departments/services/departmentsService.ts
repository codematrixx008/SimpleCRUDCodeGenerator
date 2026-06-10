import type { Department } from "../models/Department";
import type { CreateDepartmentRequest } from "../models/CreateDepartmentRequest";
import type { UpdateDepartmentRequest } from "../models/UpdateDepartmentRequest";
import { departmentsApi } from "../api/departmentsApi";

export const departmentsService = {
  getAll(): Promise<Department[]> {
    return departmentsApi.getAll();
  },

  getById(id: number): Promise<Department> {
    return departmentsApi.getById(id);
  },

  create(payload: CreateDepartmentRequest): Promise<Department> {
    return departmentsApi.create(payload);
  },

  update(id: number, payload: UpdateDepartmentRequest): Promise<void> {
    return departmentsApi.update(id, payload);
  },

  delete(id: number): Promise<void> {
    return departmentsApi.delete(id);
  }
};
