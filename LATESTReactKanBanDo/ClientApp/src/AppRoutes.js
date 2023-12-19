import React from 'react';
import Login from "./components/Login";
import Register from "./components/Register";
import { Home } from "./components/Home";
import MainPage from './components/MainPage';
import ProtectedRoute from './components/ProtectedRoute';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/login',
      element: <Login />
  },
  {
    path: '/register',
      element: <Register />
  },
  {
    path: '/mainpage',
    element: (
        <ProtectedRoute>
            <MainPage />
        </ProtectedRoute>
    )
  }
];

export default AppRoutes;
