import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

//Injeta o token em toda requisiçao autenticada
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('@App:token');
  
  //Nao envia token para rotas de autenticacao
  const isAuthRoute = config.url?.includes('/auth/login') || config.url?.includes('/auth/register');

  if (token && !isAuthRoute) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  const lang = localStorage.getItem('@App:language') || 'pt-BR';
  config.headers['Accept-Language'] = lang;

  return config;
});

export default api;
