import type { Employee } from "../models/Employee";
import type { CreateEmployeeRequest } from "../models/CreateEmployeeRequest";
import type { UpdateEmployeeRequest } from "../models/UpdateEmployeeRequest";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";
const BASE_URL = `${API_BASE_URL}/api/Employees`;

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, options);

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Request failed with status ${response.status}`);
  }

  return (await response.json()) as T;
}

async function requestNoContent(url: string, options?: RequestInit): Promise<void> {
  const response = await fetch(url, options);

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Request failed with status ${response.status}`);
  }
}

export const employeesApi = {
  getAll(): Promise<Employee[]> {
    return request<Employee[]>(BASE_URL);
  },

  getById(id: number): Promise<Employee> {
    return request<Employee>(`${BASE_URL}/${id}`);
  },

  create(payload: CreateEmployeeRequest): Promise<Employee> {
    return request<Employee>(BASE_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
  },

  update(id: number, payload: UpdateEmployeeRequest): Promise<void> {
    return requestNoContent(`${BASE_URL}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
  },

  delete(id: number): Promise<void> {
    return requestNoContent(`${BASE_URL}/${id}`, {
      method: "DELETE"
    });
  }
};
