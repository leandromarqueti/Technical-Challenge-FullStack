import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { useTheme } from '../../contexts/ThemeContext';
import { Moon, Sun } from 'lucide-react';
import styles from './styles.module.css';

const loginSchema = z.object({
  email: z.string().min(1, 'O e-mail é obrigatório').email('Formato de e-mail inválido'),
  password: z.string().min(6, 'A senha deve conter pelo menos 6 caracteres')
});

type LoginFormInputs = z.infer<typeof loginSchema>;

export const Login: React.FC = () => {
  const { login } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<LoginFormInputs>({
    resolver: zodResolver(loginSchema)
  });

  const onSubmit = async (data: LoginFormInputs) => {
    setError(null);
    setLoading(true);
    try {
      await login(data.email, data.password);
      navigate('/dashboard');
    } catch (err: any) {
      const data = err.response?.data;
      const msg = data?.message || data?.Message || 'E-mail ou senha inválidos. Verifique suas credenciais.';
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.brandPanel}>
        <h1>Technical Challenge</h1>
        <p>Desafio Técnico Full Stack: Sistema de gestão financeira para controle de transações, pessoas e categorias.</p>
      </div>

      <div className={styles.loginPanel}>
        <button onClick={toggleTheme} className={styles.themeToggle} aria-label="Toggle Theme">
          {theme === 'light' ? <Moon size={20} /> : <Sun size={20} />}
        </button>

        <div className={`${styles.formBox} glass-panel`}>
          <div className={styles.headerInfo}>
            <h2>Bem-vindo</h2>
            <p>Acesse o sistema do desafio técnico.</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className={styles.formBox} style={{ padding: 0 }}>
            <div className={styles.inputGroup}>
              <label>E-mail</label>
              <input
                type="email"
                placeholder="nome@empresa.com"
                {...register('email')}
              />
              {errors.email && <span className={styles.errorMessage}>{errors.email.message}</span>}
            </div>

            <div className={styles.inputGroup}>
              <label>Senha</label>
              <input
                type="password"
                placeholder="••••••••"
                {...register('password')}
              />
              {errors.password && <span className={styles.errorMessage}>{errors.password.message}</span>}
            </div>

            {error && <span className={styles.errorMessage}>{error}</span>}

            <button type="submit" className={styles.submitBtn} disabled={loading}>
              {loading ? 'Entrando...' : 'Acessar Plataforma'}
            </button>

            <p style={{ textAlign: 'center', fontSize: '0.9rem', marginTop: '0.5rem' }}>
              Não tem conta? <Link to="/register">Cadastre-se</Link>
            </p>
          </form>
        </div>
      </div>
    </div>
  );
};
