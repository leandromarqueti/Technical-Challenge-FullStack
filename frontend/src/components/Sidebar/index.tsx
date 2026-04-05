import React from 'react';
import { NavLink } from 'react-router-dom';
import { 
  LayoutDashboard, 
  Users, 
  Tags, 
  ArrowRightLeft, 
  LogOut 
} from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import styles from './styles.module.css';

export const Sidebar: React.FC = () => {
  const { logout } = useAuth();

  return (
    <aside className={styles.sidebar}>
      <div className={styles.logo}>
        <h3>Technical Challenge</h3>
      </div>

      <nav className={styles.nav}>
        <NavLink 
          to="/dashboard" 
          className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
        >
          <LayoutDashboard size={20} />
          <span>Dashboard</span>
        </NavLink>

        <NavLink 
          to="/persons" 
          className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
        >
          <Users size={20} />
          <span>Pessoas</span>
        </NavLink>

        <NavLink 
          to="/categories" 
          className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
        >
          <Tags size={20} />
          <span>Categorias</span>
        </NavLink>

        <NavLink 
          to="/transactions" 
          className={({ isActive }) => isActive ? `${styles.link} ${styles.active}` : styles.link}
        >
          <ArrowRightLeft size={20} />
          <span>Transações</span>
        </NavLink>
      </nav>

      <button onClick={logout} className={styles.logoutBtn}>
        <LogOut size={20} />
        <span>Sair</span>
      </button>
    </aside>
  );
};
