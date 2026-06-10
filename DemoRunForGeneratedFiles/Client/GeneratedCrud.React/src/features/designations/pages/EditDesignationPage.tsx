import { FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { UpdateDesignationRequest } from "../models/UpdateDesignationRequest";
import { DesignationForm } from "../components/DesignationForm";
import { designationsService } from "../services/designationsService";

export function EditDesignationPage() {
  const { id: idParam } = useParams();
  const id = Number(idParam);
  const navigate = useNavigate();
  const [form, setForm] = useState<UpdateDesignationRequest | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!idParam || Number.isNaN(id)) {
      setError("Invalid Designation id.");
      setIsLoading(false);
      return;
    }

    let isMounted = true;

    designationsService.getById(id)
      .then((designation) => {
        if (isMounted) {
          setForm({
        designationName: designation.designationName ?? "",
        designationCode: designation.designationCode ?? "",
        description: designation.description ?? null
          });
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Designation.");
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
    } as UpdateDesignationRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form) {
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await designationsService.update(id, form);
      navigate("/designations");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to update Designation.");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (isLoading) {
    return <p>Loading Designation...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  if (!form) {
    return <p>Designation not found.</p>;
  }

  return (
    <section>
      <h1>Edit Designation</h1>
      <DesignationForm
        value={form}
        submitText="Save"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
