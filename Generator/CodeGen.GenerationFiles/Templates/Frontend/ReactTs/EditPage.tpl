import { FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { Update{{EntityName}}Request } from "../models/Update{{EntityName}}Request";
import { {{EntityName}}Form } from "../components/{{EntityName}}Form";
import { {{EntityPluralVariable}}Service } from "../services/{{EntityPluralVariable}}Service";

export function Edit{{EntityName}}Page() {
  const { id: idParam } = useParams();
  const id = {{RouteKeyExpression}};
  const navigate = useNavigate();
  const [form, setForm] = useState<Update{{EntityName}}Request | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if ({{InvalidRouteKeyCondition}}) {
      setError("Invalid {{EntityName}} id.");
      setIsLoading(false);
      return;
    }

    let isMounted = true;

    {{EntityPluralVariable}}Service.getById(id)
      .then(({{EntityVariable}}) => {
        if (isMounted) {
          setForm({
{{EditFormStateAssignments}}
          });
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load {{EntityName}}.");
        }
      })
      .finally(() => {
        if (isMounted) {
          setIsLoading(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, [id, idParam]);

  function handleChange(field: string, value: string | number | boolean | null) {
    setForm((current) => ({
      ...current,
      [field]: value
    } as Update{{EntityName}}Request));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form) {
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await {{EntityPluralVariable}}Service.update(id, form);
      navigate("/{{RouteSegment}}");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to update {{EntityName}}.");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (isLoading) {
    return <p>Loading {{EntityName}}...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  if (!form) {
    return <p>{{EntityName}} not found.</p>;
  }

  return (
    <section>
      <h1>Edit {{EntityName}}</h1>
      <{{EntityName}}Form
        value={form}
        submitText="Save"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
