import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const ProtectedRoute: React.FC = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    //manda pro login se não tiver autenticado
    return <Navigate to="/login" replace />;
  }

  //renderiza as rotas filhas se estiver logado
  return <Outlet />;
};
