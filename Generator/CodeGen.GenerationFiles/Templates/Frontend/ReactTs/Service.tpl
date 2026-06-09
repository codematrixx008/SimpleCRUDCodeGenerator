import type { {{EntityName}} } from "../models/{{EntityName}}";
import type { Create{{EntityName}}Request } from "../models/Create{{EntityName}}Request";
import type { Update{{EntityName}}Request } from "../models/Update{{EntityName}}Request";
import { {{EntityPluralVariable}}Api } from "../api/{{EntityPluralVariable}}Api";

export const {{EntityPluralVariable}}Service = {
  getAll(): Promise<{{EntityName}}[]> {
    return {{EntityPluralVariable}}Api.getAll();
  },

  getById(id: {{KeyTypeTs}}): Promise<{{EntityName}}> {
    return {{EntityPluralVariable}}Api.getById(id);
  },

  create(payload: Create{{EntityName}}Request): Promise<{{EntityName}}> {
    return {{EntityPluralVariable}}Api.create(payload);
  },

  update(id: {{KeyTypeTs}}, payload: Update{{EntityName}}Request): Promise<void> {
    return {{EntityPluralVariable}}Api.update(id, payload);
  },

  delete(id: {{KeyTypeTs}}): Promise<void> {
    return {{EntityPluralVariable}}Api.delete(id);
  }
};
