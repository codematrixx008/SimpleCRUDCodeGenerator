import type { FormEvent } from "react";
import type { Create{{EntityName}}Request } from "../models/Create{{EntityName}}Request";
import type { Update{{EntityName}}Request } from "../models/Update{{EntityName}}Request";

type {{EntityName}}FormValue = Partial<Create{{EntityName}}Request & Update{{EntityName}}Request>;
type {{EntityName}}FormFieldValue = string | number | boolean | null;

interface {{EntityName}}FormProps {
  value: {{EntityName}}FormValue;
  submitText: string;
  isSubmitting?: boolean;
  onChange: (field: string, value: {{EntityName}}FormFieldValue) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
}

export function {{EntityName}}Form({ value, submitText, isSubmitting = false, onChange, onSubmit }: {{EntityName}}FormProps) {
  return (
    <form onSubmit={onSubmit} className="crud-form">
{{FormInputs}}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : submitText}
      </button>
    </form>
  );
}
