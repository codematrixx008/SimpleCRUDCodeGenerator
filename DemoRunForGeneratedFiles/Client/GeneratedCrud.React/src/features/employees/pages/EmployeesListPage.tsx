import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { Employee } from "../models/Employee";
import { employeesService } from "../services/employeesService";

export function EmployeesListPage() {
  const [items, setItems] = useState<Employee[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    employeesService.getAll()
      .then((data) => {
        if (isMounted) {
          setItems(data);
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Employees.");
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
    const confirmed = window.confirm("Delete this Employee?");
    if (!confirmed) {
      return;
    }

    await employeesService.delete(id);
    setItems((current) => current.filter((item) => item.id !== id));
  }

  if (isLoading) {
    return <p>Loading Employees...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  return (
    <section>
      <header className="page-header">
        <h1>Employees</h1>
        <Link to="/employees/create">Create Employee</Link>
      </header>

      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>D O B</th>
            <th>Gender</th>
            <th>Address</th>
            <th>Created Date</th>
            <th>Updated Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {items.map((employee) => (
            <tr key={String(employee.id)}>
            <td>{formatDisplayValue(employee.id)}</td>
            <td>{formatDisplayValue(employee.firstName)}</td>
            <td>{formatDisplayValue(employee.lastName)}</td>
            <td>{formatDisplayValue(employee.dOB)}</td>
            <td>{formatDisplayValue(employee.gender)}</td>
            <td>{formatDisplayValue(employee.address)}</td>
            <td>{formatDisplayValue(employee.createdDate)}</td>
            <td>{formatDisplayValue(employee.updatedDate)}</td>
              <td>
                <Link to={`/employees/${employee.id}/edit`}>Edit</Link>{" "}
                <button type="button" onClick={() => handleDelete(employee.id)}>
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
