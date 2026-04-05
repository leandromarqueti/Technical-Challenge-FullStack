import React, { useEffect, useState } from 'react';
import api from '../../services/api';
import { Loader2, TrendingUp, TrendingDown, Wallet } from 'lucide-react';
import styles from './styles.module.css';

interface DashboardData {
  totalRevenue: number;
  totalExpense: number;
  balance: number;
}

export const Dashboard: React.FC = () => {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchDashboard = async () => {
    try {
      const response = await api.get('/dashboard');
      //A API retorna Result<DashboardResponse>
      setData(response.data.data);
    } catch (err) {
      console.error('Erro ao carregar dashboard:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDashboard();
  }, []);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  return (
    <div className={styles.container}>
      <div className={styles.titleBox}>
        <h1>Visão Geral</h1>
        <p>Acompanhe os resultados financeiros e controle corporativo em tempo real.</p>
      </div>

      {loading ? (
        <div className={styles.loader}>
          <Loader2 className="spinning" size={40} />
          <p>Carregando informações...</p>
        </div>
      ) : (
        <div className={styles.grid}>
          <div className={`${styles.card} ${styles.income} glass-panel`}>
            <div className={styles.cardHeader}>
              <h3>Entradas Totais</h3>
              <TrendingUp size={24} />
            </div>
            <div className={styles.value}>{formatCurrency(data?.totalRevenue || 0)}</div>
          </div>

          <div className={`${styles.card} ${styles.expense} glass-panel`}>
            <div className={styles.cardHeader}>
              <h3>Saídas Totais</h3>
              <TrendingDown size={24} />
            </div>
            <div className={styles.value}>{formatCurrency(data?.totalExpense || 0)}</div>
          </div>

          <div className={`${styles.card} ${styles.balance} glass-panel`}>
            <div className={styles.cardHeader}>
              <h3>Saldo Líquido</h3>
              <Wallet size={24} />
            </div>
            <div className={`${styles.value} ${(data?.balance || 0) < 0 ? styles.negative : ''}`}>
              {formatCurrency(data?.balance || 0)}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
