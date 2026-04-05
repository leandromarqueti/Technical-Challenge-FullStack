import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

//configura o token no cabeçalho de toda requisição
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('@App:token');
  
  //não manda o token se for pra login ou cadastro
  const isAuthRoute = config.url?.includes('/auth/login') || config.url?.includes('/auth/register');

  if (token && !isAuthRoute) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  
  const lang = localStorage.getItem('@App:language') || 'pt-BR';
  config.headers['Accept-Language'] = lang;

  return config;
});

//trata erros de resposta (como o 401) globalmente
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      console.error('[API] Erro de autenticação: login necessário.');
      //opcional: limpa o storage se o token cair
      //localStorage.removeItem('@App:token');
    }
    return Promise.reject(error);
  }
);

export default api;
