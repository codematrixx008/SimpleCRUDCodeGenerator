import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { CreateDesignationRequest } from "../models/CreateDesignationRequest";
import { DesignationForm } from "../components/DesignationForm";
import { designationsService } from "../services/designationsService";

const initialFormState: CreateDesignationRequest = {
  designationName: "",
  designationCode: "",
  description: null
};

export function CreateDesignationPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState<CreateDesignationRequest>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(field: string, value: string | number | boolean | null) {
    setForm((current) => ({
      ...current,
      [field]: value
    } as CreateDesignationRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await designationsService.create(form);
      navigate("/designations");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to create Designation.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section>
      <h1>Create Designation</h1>
      {error && <p role="alert">{error}</p>}
      <DesignationForm
        value={form}
        submitText="Create"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
