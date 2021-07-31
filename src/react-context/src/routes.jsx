import React from "react";
import { Navigate, useRoutes } from "react-router-dom";

// layouts
import DashboardLayout from "./layouts/dashboard";
import LogoOnlyLayout from "./layouts/LogoOnlyLayout";

// pages
import Dashboard from "./pages/Dashboard";
import Users from "./pages/Users";
import Tasks from "./pages/Tasks";
import NotFound from "./pages/NotFound";
import UserEdit from "./pages/UserEdit";
import TaskEdit from "./pages/TaskEdit";
import About from "./pages/About";

export default function Router() {
  return useRoutes([
    {
      path: "/dashboard",
      element: <DashboardLayout />,
      children: [
        { path: "/", element: <Navigate to="/dashboard/app" replace /> },
        { path: "app", element: <Dashboard /> },
        { path: "users", element: <Users /> },
        { path: "users/:id", element: <UserEdit /> },
        { path: "tasks", element: <Tasks /> },
        { path: "tasks/:id", element: <TaskEdit /> },
      ],
    },
    {
      path: "/",
      element: <LogoOnlyLayout />,
      children: [
        { path: "404", element: <NotFound /> },
        { path: "/", element: <Navigate to="/dashboard" /> },
        { path: "about", element: <About /> },
        { path: "*", element: <Navigate to="/404" /> },
      ],
    },

    { path: "*", element: <Navigate to="/404" replace /> },
  ]);
}
