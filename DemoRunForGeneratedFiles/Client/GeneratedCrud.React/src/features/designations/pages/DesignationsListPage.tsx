import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { Designation } from "../models/Designation";
import { designationsService } from "../services/designationsService";

export function DesignationsListPage() {
  const [items, setItems] = useState<Designation[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    designationsService.getAll()
      .then((data) => {
        if (isMounted) {
          setItems(data);
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load Designations.");
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
    const confirmed = window.confirm("Delete this Designation?");
    if (!confirmed) {
      return;
    }

    await designationsService.delete(id);
    setItems((current) => current.filter((item) => item.id !== id));
  }

  if (isLoading) {
    return <p>Loading Designations...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  return (
    <section>
      <header className="page-header">
        <h1>Designations</h1>
        <Link to="/designations/create">Create Designation</Link>
      </header>

      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Designation Name</th>
            <th>Designation Code</th>
            <th>Description</th>
            <th>Created Date</th>
            <th>Updated Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {items.map((designation) => (
            <tr key={String(designation.id)}>
            <td>{formatDisplayValue(designation.id)}</td>
            <td>{formatDisplayValue(designation.designationName)}</td>
            <td>{formatDisplayValue(designation.designationCode)}</td>
            <td>{formatDisplayValue(designation.description)}</td>
            <td>{formatDisplayValue(designation.createdDate)}</td>
            <td>{formatDisplayValue(designation.updatedDate)}</td>
              <td>
                <Link to={`/designations/${designation.id}/edit`}>Edit</Link>{" "}
                <button type="button" onClick={() => handleDelete(designation.id)}>
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
