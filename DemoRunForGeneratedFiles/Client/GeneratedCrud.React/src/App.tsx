import { BrowserRouter, Link, Navigate, Route, Routes } from "react-router-dom";
import { EmployeesListPage } from "./features/employees/pages/EmployeesListPage";
import { CreateEmployeePage } from "./features/employees/pages/CreateEmployeePage";
import { EditEmployeePage } from "./features/employees/pages/EditEmployeePage";
import { DepartmentsListPage } from "./features/departments/pages/DepartmentsListPage";
import { CreateDepartmentPage } from "./features/departments/pages/CreateDepartmentPage";
import { EditDepartmentPage } from "./features/departments/pages/EditDepartmentPage";
import { DesignationsListPage } from "./features/designations/pages/DesignationsListPage";
import { CreateDesignationPage } from "./features/designations/pages/CreateDesignationPage";
import { EditDesignationPage } from "./features/designations/pages/EditDesignationPage";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <div className="app-shell">
        <header>
          <h1>Simple Employee CRUD</h1>

          <nav>
            <Link to="/employees">Employees</Link>
            <Link to="/departments">Departments</Link>
            <Link to="/designations">Designations</Link>
          </nav>
        </header>

        <main>
          <Routes>
            <Route path="/" element={<Navigate to="/employees" replace />} />

            <Route path="/employees" element={<EmployeesListPage />} />
            <Route path="/employees/create" element={<CreateEmployeePage />} />
            <Route path="/employees/:id/edit" element={<EditEmployeePage />} />

            <Route path="/departments" element={<DepartmentsListPage />} />
            <Route path="/departments/create" element={<CreateDepartmentPage />} />
            <Route path="/departments/:id/edit" element={<EditDepartmentPage />} />
            

            <Route path="/designations" element={<DesignationsListPage />} />
            <Route path="/designations/create" element={<CreateDesignationPage />} />
            <Route path="/designations/:id/edit" element={<EditDesignationPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;