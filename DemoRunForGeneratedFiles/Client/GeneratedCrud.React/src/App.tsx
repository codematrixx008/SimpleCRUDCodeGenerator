import { BrowserRouter, Link, Navigate, Route, Routes } from "react-router-dom";
import { EmployeesListPage } from "./features/employees/pages/EmployeesListPage";
import { CreateEmployeePage } from "./features/employees/pages/CreateEmployeePage";
import { EditEmployeePage } from "./features/employees/pages/EditEmployeePage";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <div className="app-shell">
        <header>
          <h1>Simple Employee CRUD</h1>

          <nav>
            <Link to="/employees">Employees</Link>
          </nav>
        </header>

        <main>
          <Routes>
            <Route path="/" element={<Navigate to="/employees" replace />} />
            <Route path="/employees" element={<EmployeesListPage />} />
            <Route path="/employees/create" element={<CreateEmployeePage />} />
            <Route path="/employees/:id/edit" element={<EditEmployeePage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;