import type { {{EntityName}} } from "../models/{{EntityName}}";
import type { Create{{EntityName}}Request } from "../models/Create{{EntityName}}Request";
import type { Update{{EntityName}}Request } from "../models/Update{{EntityName}}Request";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";
const BASE_URL = `${API_BASE_URL}{{ApiRoute}}`;

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

export const {{EntityPluralVariable}}Api = {
  getAll(): Promise<{{EntityName}}[]> {
    return request<{{EntityName}}[]>(BASE_URL);
  },

  getById(id: {{KeyTypeTs}}): Promise<{{EntityName}}> {
    return request<{{EntityName}}>(`${BASE_URL}/${id}`);
  },

  create(payload: Create{{EntityName}}Request): Promise<{{EntityName}}> {
    return request<{{EntityName}}>(BASE_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
  },

  update(id: {{KeyTypeTs}}, payload: Update{{EntityName}}Request): Promise<void> {
    return requestNoContent(`${BASE_URL}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
  },

  delete(id: {{KeyTypeTs}}): Promise<void> {
    return requestNoContent(`${BASE_URL}/${id}`, {
      method: "DELETE"
    });
  }
};
