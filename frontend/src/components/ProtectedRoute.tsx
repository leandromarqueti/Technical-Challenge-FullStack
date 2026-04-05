import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const ProtectedRoute: React.FC = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    //Redirect to the login page if not authenticated
    return <Navigate to="/login" replace />;
  }

  //Render child routes if authenticated
  return <Outlet />;
};
