import { FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { UpdateEmployeeRequest } from "../models/UpdateEmployeeRequest";
import { EmployeeForm } from "../components/EmployeeForm";
import { employeesService } from "../services/employeesService";

export function EditEmployeePage() {
  const { id: idParam } = useParams();
  const id = Number(idParam);
  const navigate = useNavigate();
  const [form, setForm] = useState<UpdateEmployeeRequest | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!idParam || Number.isNaN(id)) {
      setError("Invalid Employee id.");
      setIsLoading(false);
      return;
    }

    let isMounted = true;

    employeesService.getById(id)
      .then((employee) => {
        if (isMounted) {
          setForm({
        firstName: employee.firstName ?? "",
        lastName: employee.lastName ?? "",
        dOB: employee.dob ? employee.dob.substring(0, 10) : "",
        gender: employee.gender ?? "",
        address: employee.address ?? null,
        departmentId: employee.departmentId ?? null
          });
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Employee.");
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
    } as UpdateEmployeeRequest));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form) {
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await employeesService.update(id, form);
      navigate("/employees");
    } catch (exception: unknown) {
      setError(exception instanceof Error ? exception.message : "Failed to update Employee.");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (isLoading) {
    return <p>Loading Employee...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  if (!form) {
    return <p>Employee not found.</p>;
  }

  return (
    <section>
      <h1>Edit Employee</h1>
      <EmployeeForm
        value={form}
        submitText="Save"
        isSubmitting={isSubmitting}
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </section>
  );
}
