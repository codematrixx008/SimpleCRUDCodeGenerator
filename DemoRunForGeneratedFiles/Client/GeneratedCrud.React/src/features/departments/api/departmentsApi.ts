import type { Department } from "../models/Department";
import type { CreateDepartmentRequest } from "../models/CreateDepartmentRequest";
import type { UpdateDepartmentRequest } from "../models/UpdateDepartmentRequest";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";
const BASE_URL = `${API_BASE_URL}/api/Departments`;

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

export const departmentsApi = {
  getAll(): Promise<Department[]> {
    return request<Department[]>(BASE_URL);
  },

  getById(id: number): Promise<Department> {
    return request<Department>(`${BASE_URL}/${id}`);
  },

  create(payload: CreateDepartmentRequest): Promise<Department> {
    return request<Department>(BASE_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
  },

  update(id: number, payload: UpdateDepartmentRequest): Promise<void> {
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
