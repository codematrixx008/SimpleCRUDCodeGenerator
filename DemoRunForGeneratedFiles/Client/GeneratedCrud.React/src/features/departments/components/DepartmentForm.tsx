import type { FormEvent } from "react";
import type { CreateDepartmentRequest } from "../models/CreateDepartmentRequest";
import type { UpdateDepartmentRequest } from "../models/UpdateDepartmentRequest";

type DepartmentFormValue = Partial<CreateDepartmentRequest & UpdateDepartmentRequest>;
type DepartmentFormFieldValue = string | number | boolean | null;

interface DepartmentFormProps {
  value: DepartmentFormValue;
  submitText: string;
  isSubmitting?: boolean;
  onChange: (field: string, value: DepartmentFormFieldValue) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export function DepartmentForm({ value, submitText, isSubmitting = false, onChange, onSubmit }: DepartmentFormProps) {
  return (
    <form onSubmit={onSubmit} className="crud-form">
      <div className="form-field">
        <label htmlFor="departmentName">Department Name</label>
        <input
          id="departmentName"
          name="departmentName"
          type="text"
          value={value.departmentName ?? ""}
          onChange={(event) => onChange("departmentName", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="departmentCode">Department Code</label>
        <input
          id="departmentCode"
          name="departmentCode"
          type="text"
          value={value.departmentCode ?? ""}
          onChange={(event) => onChange("departmentCode", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="description">Description</label>
        <textarea
          id="description"
          name="description"
          value={value.description ?? ""}
          onChange={(event) => onChange("description", event.target.value === "" ? null : event.target.value)}
        />
      </div>

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitText}
      </button>
    </form>
  );
}
