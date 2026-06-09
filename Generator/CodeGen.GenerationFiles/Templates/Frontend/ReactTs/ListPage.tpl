import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { {{EntityName}} } from "../models/{{EntityName}}";
import { {{EntityPluralVariable}}Service } from "../services/{{EntityPluralVariable}}Service";

export function {{EntityPlural}}ListPage() {
  const [items, setItems] = useState<{{EntityName}}[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    {{EntityPluralVariable}}Service.getAll()
      .then((data) => {
        if (isMounted) {
          setItems(data);
        }
      })
      .catch((exception: unknown) => {
        if (isMounted) {
          setError(exception instanceof Error ? exception.message : "Failed to load {{EntityPlural}}.");
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

  async function handleDelete(id: {{KeyTypeTs}}) {
    const confirmed = window.confirm("Delete this {{EntityName}}?");
    if (!confirmed) {
      return;
    }

    await {{EntityPluralVariable}}Service.delete(id);
    setItems((current) => current.filter((item) => item.{{KeyVariable}} !== id));
  }

  if (isLoading) {
    return <p>Loading {{EntityPlural}}...</p>;
  }

  if (error) {
    return <p role="alert">{error}</p>;
  }

  return (
    <section>
      <header className="page-header">
        <h1>{{EntityPlural}}</h1>
        <Link to="/{{RouteSegment}}/create">Create {{EntityName}}</Link>
      </header>

      <table>
        <thead>
          <tr>
{{TableHeaders}}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {items.map(({{EntityVariable}}) => (
            <tr key={String({{EntityVariable}}.{{KeyVariable}})}>
{{TableCells}}
              <td>
                <Link to={`/{{RouteSegment}}/${{{EntityVariable}}.{{KeyVariable}}}/edit`}>Edit</Link>{" "}
                <button type="button" onClick={() => handleDelete({{EntityVariable}}.{{KeyVariable}})}>
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
