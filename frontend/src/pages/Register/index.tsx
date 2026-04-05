import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useAuth } from '../../contexts/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { useTheme } from '../../contexts/ThemeContext';
import { Moon, Sun } from 'lucide-react';
import styles from '../Login/styles.module.css';

const registerSchema = z.object({
  name: z.string().min(2, 'O nome deve ter pelo menos 2 caracteres'),
  email: z.string().min(1, 'O e-mail é obrigatório').email('Formato de e-mail inválido'),
  password: z.string().min(6, 'A senha deve ter pelo menos 6 caracteres'),
});

type RegisterFormInputs = z.infer<typeof registerSchema>;

export const Register: React.FC = () => {
  const { register: registerUser } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<RegisterFormInputs>({
    resolver: zodResolver(registerSchema)
  });

  const onSubmit = async (data: RegisterFormInputs) => {
    setError(null);
    setLoading(true);
    try {
      await registerUser(data.name, data.email, data.password);
      navigate('/dashboard');
    } catch (err: any) {
      const data = err.response?.data;
      const message = data?.Message || data?.message || 'Não foi possível criar a conta. Tente novamente mais tarde.';
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.brandPanel}>
        <h1>Technical Challenge</h1>
        <p>Desafio Técnico Full Stack: Crie sua conta para testar o sistema de gestão financeira.</p>
      </div>

      <div className={styles.loginPanel}>
        <button onClick={toggleTheme} className={styles.themeToggle} aria-label="Toggle Theme">
          {theme === 'light' ? <Moon size={20} /> : <Sun size={20} />}
        </button>

        <div className={`${styles.formBox} glass-panel`}>
          <div className={styles.headerInfo}>
            <h2>Criar conta</h2>
            <p>Preencha os dados para acessar a plataforma.</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className={styles.formBox} style={{ padding: 0 }}>
            <div className={styles.inputGroup}>
              <label>Nome Completo</label>
              <input type="text" placeholder="Seu nome" {...register('name')} />
              {errors.name && <span className={styles.errorMessage}>{errors.name.message}</span>}
            </div>

            <div className={styles.inputGroup}>
              <label>E-mail</label>
              <input type="email" placeholder="nome@empresa.com" {...register('email')} />
              {errors.email && <span className={styles.errorMessage}>{errors.email.message}</span>}
            </div>

            <div className={styles.inputGroup}>
              <label>Senha</label>
              <input type="password" placeholder="••••••••" {...register('password')} />
              {errors.password && <span className={styles.errorMessage}>{errors.password.message}</span>}
            </div>

            {error && <span className={styles.errorMessage}>{error}</span>}

            <button type="submit" className={styles.submitBtn} disabled={loading}>
              {loading ? 'Criando conta...' : 'Criar conta'}
            </button>

            <p style={{ textAlign: 'center', fontSize: '0.9rem', marginTop: '0.5rem' }}>
              Já tem conta? <Link to="/login">Faça login</Link>
            </p>
          </form>
        </div>
      </div>
    </div>
  );
};
