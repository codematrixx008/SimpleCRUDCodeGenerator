import type { Designation } from "../models/Designation";
import type { CreateDesignationRequest } from "../models/CreateDesignationRequest";
import type { UpdateDesignationRequest } from "../models/UpdateDesignationRequest";
import { designationsApi } from "../api/designationsApi";

export const designationsService = {
  getAll(): Promise<Designation[]> {
    return designationsApi.getAll();
  },

  getById(id: number): Promise<Designation> {
    return designationsApi.getById(id);
  },

  create(payload: CreateDesignationRequest): Promise<Designation> {
    return designationsApi.create(payload);
  },

  update(id: number, payload: UpdateDesignationRequest): Promise<void> {
    return designationsApi.update(id, payload);
  },

  delete(id: number): Promise<void> {
    return designationsApi.delete(id);
  }
};
