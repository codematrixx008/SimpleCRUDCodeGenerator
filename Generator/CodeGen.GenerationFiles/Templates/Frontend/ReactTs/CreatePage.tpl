import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { Create{{EntityName}}Request } from "../models/Create{{EntityName}}Request";
import { {{EntityName}}Form } from "../components/{{EntityName}}Form";
import { {{EntityPluralVariable}}Service } from "../services/{{EntityPluralVariable}}Service";

const initialFormState: Create{{EntityName}}Request = {
{{InitialFormState}}
};

export function Create{{EntityName}}Page() {
  const navigate = useNavigate();
  const [form, setForm] = useState<Create{{EntityName}}Request>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(field: string, value: string | number | boolean | null) {
    setForm((current) => ({
      ...current,
      [field]: value
    } as Create{{EntityName}}Request));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await {{EntityPluralVariable}}Service.create(form);
      navigate("/{{RouteSegment}}");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to create {{EntityName}}.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section>
      <h1>Create {{EntityName}}</h1>
      {error && <p role="alert">{error}</p>}
      <{{EntityName}}Form
        value={form}
        submitText="Create"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
