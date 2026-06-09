import type { FormEvent } from "react";
import type { CreateEmployeeRequest } from "../models/CreateEmployeeRequest";
import type { UpdateEmployeeRequest } from "../models/UpdateEmployeeRequest";

type EmployeeFormValue = Partial<CreateEmployeeRequest & UpdateEmployeeRequest>;
type EmployeeFormFieldValue = string | number | boolean | null;

interface EmployeeFormProps {
  value: EmployeeFormValue;
  submitText: string;
  isSubmitting?: boolean;
  onChange: (field: string, value: EmployeeFormFieldValue) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export function EmployeeForm({ value, submitText, isSubmitting = false, onChange, onSubmit }: EmployeeFormProps) {
  return (
    <form onSubmit={onSubmit} className="crud-form">
      <div className="form-field">
        <label htmlFor="firstName">First Name</label>
        <input
          id="firstName"
          name="firstName"
          type="text"
          value={value.firstName ?? ""}
          onChange={(event) => onChange("firstName", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="lastName">Last Name</label>
        <input
          id="lastName"
          name="lastName"
          type="text"
          value={value.lastName ?? ""}
          onChange={(event) => onChange("lastName", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="dOB">D O B</label>
        <input
          id="dOB"
          name="dOB"
          type="date"
          value={(value.dOB ?? "").substring(0, 10)}
          onChange={(event) => onChange("dOB", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="gender">Gender</label>
        <input
          id="gender"
          name="gender"
          type="text"
          value={value.gender ?? ""}
          onChange={(event) => onChange("gender", event.target.value)}
        />
      </div>
      <div className="form-field">
        <label htmlFor="address">Address</label>
        <textarea
          id="address"
          name="address"
          value={value.address ?? ""}
          onChange={(event) => onChange("address", event.target.value === "" ? null : event.target.value)}
        />
      </div>

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitText}
      </button>
    </form>
  );
}
