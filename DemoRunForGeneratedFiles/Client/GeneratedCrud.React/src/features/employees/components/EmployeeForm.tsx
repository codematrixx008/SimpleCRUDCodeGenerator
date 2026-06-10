import { type FormEvent, useEffect, useState } from "react";
import type { CreateEmployeeRequest } from "../models/CreateEmployeeRequest";
import type { UpdateEmployeeRequest } from "../models/UpdateEmployeeRequest";
import type { Department } from "../../departments/models/Department";
import { departmentsService } from "../../departments/services/departmentsService";
import type { Designation } from "../../designations/models/Designation";
import { designationsService } from "../../designations/services/designationsService";

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
  const [departments, setDepartments] = useState<Department[]>([]);
  const [isLoadingDepartments, setIsLoadingDepartments] = useState(false);

  useEffect(() => {
    let isMounted = true;
    setIsLoadingDepartments(true);

    departmentsService.getAll()
      .then((data) => {
        if (isMounted) {
          setDepartments(data);
        }
      })
      .catch((exception: unknown) => {
        console.error("Failed to load Departments.", exception);
      })
      .finally(() => {
        if (isMounted) {
          setIsLoadingDepartments(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, []);

  const [designations, setDesignations] = useState<Designation[]>([]);
  const [isLoadingDesignations, setIsLoadingDesignations] = useState(false);

  useEffect(() => {
    let isMounted = true;
    setIsLoadingDesignations(true);

    designationsService.getAll()
      .then((data) => {
        if (isMounted) {
          setDesignations(data);
        }
      })
      .catch((exception: unknown) => {
        console.error("Failed to load Designations.", exception);
      })
      .finally(() => {
        if (isMounted) {
          setIsLoadingDesignations(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, []);

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
          value={(value.dob ?? "").substring(0, 10)}
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
      <div className="form-field">
        <label htmlFor="departmentId">Department</label>
        <select
          id="departmentId"
          name="departmentId"
          value={value.departmentId ?? ""}
          onChange={(event) => onChange("departmentId", event.target.value === "" ? null : Number(event.target.value))}
          disabled={isSubmitting || isLoadingDepartments}
        >
          <option value="">Select Department</option>
          {departments.map((department) => (
            <option key={department.id} value={department.id}>
              {department.departmentName}
            </option>
          ))}
        </select>
      </div>
      <div className="form-field">
        <label htmlFor="designationId">Designation</label>
        <select
          id="designationId"
          name="designationId"
          value={value.designationId ?? ""}
          onChange={(event) => onChange("designationId", event.target.value === "" ? null : Number(event.target.value))}
          disabled={isSubmitting || isLoadingDesignations}
        >
          <option value="">Select Designation</option>
          {designations.map((designation) => (
            <option key={designation.id} value={designation.id}>
              {designation.designationName}
            </option>
          ))}
        </select>
      </div>

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitText}
      </button>
    </form>
  );
}
