Frontend setup notes for {{FrontendAppName}}

Required package if your app does not already use routing:

npm install react-router-dom

Optional environment variable for the backend API base URL:

VITE_API_BASE_URL=https://localhost:5001

The generated API client calls:

{{ApiRoute}}

If your ASP.NET API runs at http://localhost:5144, use:

VITE_API_BASE_URL=http://localhost:5144
