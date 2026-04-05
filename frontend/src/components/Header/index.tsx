import React from 'react';
import { useTheme } from '../../contexts/ThemeContext';
import { useAuth } from '../../contexts/AuthContext';
import { Sun, Moon } from 'lucide-react';
import styles from './styles.module.css';

export const Header: React.FC = () => {
  const { theme, toggleTheme } = useTheme();
  const { language, changeLanguage, user } = useAuth();

  return (
    <header className={styles.header}>
      <div className={styles.userInfo}>
        <span>Bem-vindo, <strong>{user?.name.split(' ')[0]}</strong></span>
      </div>

      <div className={styles.controls}>
        <select 
          className={styles.langSelect} 
          value={language} 
          onChange={(e) => changeLanguage(e.target.value)}
        >
          <option value="pt-BR">🇧🇷 PT-BR</option>
          <option value="en-US">🇺🇸 EN-US</option>
          <option value="es-ES">🇪🇸 ES</option>
        </select>

        <button onClick={toggleTheme} className={styles.themeToggle} aria-label="Toggle Theme">
          {theme === 'light' ? <Moon size={20} /> : <Sun size={20} />}
        </button>
      </div>
    </header>
  );
};
