import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:63272/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

//Injeta o token em toda requisiçao autenticada
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('@App:token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  const lang = localStorage.getItem('@App:language') || 'pt-BR';
  config.headers['Accept-Language'] = lang;

  return config;
});

export default api;
