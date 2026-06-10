import type { FormEvent } from "react";
import type { CreateDesignationRequest } from "../models/CreateDesignationRequest";
import type { UpdateDesignationRequest } from "../models/UpdateDesignationRequest";


type DesignationFormValue = Partial<CreateDesignationRequest & UpdateDesignationRequest>;
type DesignationFormFieldValue = string | number | boolean | null;

interface DesignationFormProps {
  value: DesignationFormValue;
  submitText: string;
  isSubmitting?: boolean;
  onChange: (field: string, value: DesignationFormFieldValue) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export function DesignationForm({ value, submitText, isSubmitting = false, onChange, onSubmit }: DesignationFormProps) {


  return (
    <form onSubmit={onSubmit} className="crud-form">
      <div className="form-field">
        <label htmlFor="designationName">Designation Name</label>
        <input
          id="designationName"
          name="designationName"
          type="text"
          value={value.designationName ?? ""}
          onChange={(event) => onChange("designationName", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="designationCode">Designation Code</label>
        <input
          id="designationCode"
          name="designationCode"
          type="text"
          value={value.designationCode ?? ""}
          onChange={(event) => onChange("designationCode", event.target.value)}
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
