Add these imports to your React router file, usually src/App.tsx or src/routes.tsx:

import { {{EntityPlural}}ListPage } from "./features/{{FeatureFolder}}/pages/{{EntityPlural}}ListPage";
import { Create{{EntityName}}Page } from "./features/{{FeatureFolder}}/pages/Create{{EntityName}}Page";
import { Edit{{EntityName}}Page } from "./features/{{FeatureFolder}}/pages/Edit{{EntityName}}Page";

Add these routes inside your <Routes> block:

<Route path="/{{RouteSegment}}" element={<{{EntityPlural}}ListPage />} />
<Route path="/{{RouteSegment}}/create" element={<Create{{EntityName}}Page />} />
<Route path="/{{RouteSegment}}/:id/edit" element={<Edit{{EntityName}}Page />} />

Add a menu link if you have navigation:

<Link to="/{{RouteSegment}}">{{EntityPlural}}</Link>
