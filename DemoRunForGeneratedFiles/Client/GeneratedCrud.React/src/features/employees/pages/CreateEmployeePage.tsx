import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { CreateEmployeeRequest } from "../models/CreateEmployeeRequest";
import { EmployeeForm } from "../components/EmployeeForm";
import { employeesService } from "../services/employeesService";

const initialFormState: CreateEmployeeRequest = {
  firstName: "",
  lastName: "",
  dOB: "",
  gender: "",
  address: null,
  departmentId: null
};

export function CreateEmployeePage() {
  const navigate = useNavigate();
  const [form, setForm] = useState<CreateEmployeeRequest>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(field: string, value: string | number | boolean | null) {
    setForm((current) => ({
      ...current,
      [field]: value
    } as CreateEmployeeRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await employeesService.create(form);
      navigate("/employees");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to create Employee.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section>
      <h1>Create Employee</h1>
      {error && <p role="alert">{error}</p>}
      <EmployeeForm
        value={form}
        submitText="Create"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
