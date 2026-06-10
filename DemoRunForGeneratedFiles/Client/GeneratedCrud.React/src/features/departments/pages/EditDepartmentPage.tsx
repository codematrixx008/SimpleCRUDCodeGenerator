import { FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { UpdateDepartmentRequest } from "../models/UpdateDepartmentRequest";
import { DepartmentForm } from "../components/DepartmentForm";
import { departmentsService } from "../services/departmentsService";

export function EditDepartmentPage() {
  const { id: idParam } = useParams();
  const id = Number(idParam);
  const navigate = useNavigate();
  const [form, setForm] = useState<UpdateDepartmentRequest | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!idParam || Number.isNaN(id)) {
      setError("Invalid Department id.");
      setIsLoading(false);
      return;
    }

    let isMounted = true;

    departmentsService.getById(id)
      .then((department) => {
        if (isMounted) {
          setForm({
        departmentName: department.departmentName ?? "",
        departmentCode: department.departmentCode ?? "",
        description: department.description ?? null
          });
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Department.");
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
    } as UpdateDepartmentRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form) {
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await departmentsService.update(id, form);
      navigate("/departments");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to update Department.");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (isLoading) {
    return <p>Loading Department...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  if (!form) {
    return <p>Department not found.</p>;
  }

  return (
    <section>
      <h1>Edit Department</h1>
      <DepartmentForm
        value={form}
        submitText="Save"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
