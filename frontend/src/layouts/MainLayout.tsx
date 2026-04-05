import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/Sidebar';
import { Header } from '../components/Header';
import styles from './styles.module.css';

export const MainLayout: React.FC = () => {
  return (
    <div className={styles.layout}>
      <Sidebar />
      <main className={styles.mainContent}>
        <Header />
        <div className={styles.pageBody}>
          <Outlet />
        </div>
      </main>
    </div>
  );
};
