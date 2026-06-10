import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { Department } from "../models/Department";
import { departmentsService } from "../services/departmentsService";

export function DepartmentsListPage() {
  const [items, setItems] = useState<Department[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    departmentsService.getAll()
      .then((data) => {
        if (isMounted) {
          setItems(data);
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Departments.");
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
  }, []);

  async function handleDelete(id: number) {
    const confirmed = window.confirm("Delete this Department?");
    if (!confirmed) {
      return;
    }

    await departmentsService.delete(id);
    setItems((current) => current.filter((item) => item.id !== id));
  }

  if (isLoading) {
    return <p>Loading Departments...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  return (
    <section>
      <header className="page-header">
        <h1>Departments</h1>
        <Link to="/departments/create">Create Department</Link>
      </header>

      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Department Name</th>
            <th>Department Code</th>
            <th>Description</th>
            <th>Created Date</th>
            <th>Updated Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {items.map((department) => (
            <tr key={String(department.id)}>
            <td>{formatDisplayValue(department.id)}</td>
            <td>{formatDisplayValue(department.departmentName)}</td>
            <td>{formatDisplayValue(department.departmentCode)}</td>
            <td>{formatDisplayValue(department.description)}</td>
            <td>{formatDisplayValue(department.createdDate)}</td>
            <td>{formatDisplayValue(department.updatedDate)}</td>
              <td>
                <Link to={`/departments/${department.id}/edit`}>Edit</Link>{" "}
                <button type="button" onClick={() => handleDelete(department.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

function formatDisplayValue(value: unknown): string {
  if (value === null || value === undefined || value === "") {
    return "-";
  }

  if (typeof value === "boolean") {
    return value ? "Yes" : "No";
  }

  return String(value);
}
