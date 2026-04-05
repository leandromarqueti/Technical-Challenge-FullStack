import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../services/api';

interface User {
  name: string;
  email: string;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => void;
  language: string;
  changeLanguage: (lang: string) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    const stored = localStorage.getItem('@App:user');
    return stored ? JSON.parse(stored) : null;
  });

  const [language, setLanguage] = useState<string>(() => {
    return localStorage.getItem('@App:language') || 'pt-BR';
  });

  useEffect(() => {
    localStorage.setItem('@App:language', language);
  }, [language]);

  const login = async (email: string, password: string) => {
    const { data } = await api.post('/auth/login', { email, password });
    //pega os dados dentro de data.data que é o padrão do Result
    const payload = data.data ?? data;
    localStorage.setItem('@App:token', payload.token);
    const userData = { name: payload.name, email: payload.email };
    setUser(userData);
    localStorage.setItem('@App:user', JSON.stringify(userData));
  };

  const register = async (name: string, email: string, password: string) => {
    const { data } = await api.post('/auth/register', { name, email, password });
    const payload = data.data ?? data;
    localStorage.setItem('@App:token', payload.token);
    const userData = { name: payload.name, email: payload.email };
    setUser(userData);
    localStorage.setItem('@App:user', JSON.stringify(userData));
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('@App:user');
    localStorage.removeItem('@App:token');
  };

  const changeLanguage = (lang: string) => {
    setLanguage(lang);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, login, register, logout, language, changeLanguage }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');
  return context;
};
