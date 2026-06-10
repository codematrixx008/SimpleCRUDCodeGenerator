import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { CreateDepartmentRequest } from "../models/CreateDepartmentRequest";
import { DepartmentForm } from "../components/DepartmentForm";
import { departmentsService } from "../services/departmentsService";

const initialFormState: CreateDepartmentRequest = {
  departmentName: "",
  departmentCode: "",
  description: null
};

export function CreateDepartmentPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState<CreateDepartmentRequest>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(field: string, value: string | number | boolean | null) {
    setForm((current) => ({
      ...current,
      [field]: value
    } as CreateDepartmentRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await departmentsService.create(form);
      navigate("/departments");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to create Department.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section>
      <h1>Create Department</h1>
      {error && <p role="alert">{error}</p>}
      <DepartmentForm
        value={form}
        submitText="Create"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
