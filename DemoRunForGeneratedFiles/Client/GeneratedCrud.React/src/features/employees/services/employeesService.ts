import type { Employee } from "../models/Employee";
import type { CreateEmployeeRequest } from "../models/CreateEmployeeRequest";
import type { UpdateEmployeeRequest } from "../models/UpdateEmployeeRequest";
import { employeesApi } from "../api/employeesApi";

export const employeesService = {
  getAll(): Promise<Employee[]> {
    return employeesApi.getAll();
  },

  getById(id: number): Promise<Employee> {
    return employeesApi.getById(id);
  },

  create(payload: CreateEmployeeRequest): Promise<Employee> {
    return employeesApi.create(payload);
  },

  update(id: number, payload: UpdateEmployeeRequest): Promise<void> {
    return employeesApi.update(id, payload);
  },

  delete(id: number): Promise<void> {
    return employeesApi.delete(id);
  }
};
