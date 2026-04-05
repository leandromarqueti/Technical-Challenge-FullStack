import React, { useEffect, useState } from 'react';
import api from '../../services/api';
import { Loader2, TrendingUp, TrendingDown, Wallet } from 'lucide-react';
import styles from './styles.module.css';

interface SummaryItem {
  name?: string;
  description?: string;
  totalRevenue: number;
  totalExpenses: number;
  balance: number;
}

interface DashboardData {
  totalRevenue: number;
  totalExpenses: number;
  balance: number;
  totalsByPerson: SummaryItem[];
  totalsByCategory: SummaryItem[];
}

export const Dashboard: React.FC = () => {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [fetchError, setFetchError] = useState(false);

  const fetchDashboard = async () => {
    try {
      setFetchError(false);
      const response = await api.get('/dashboard');
      setData(response.data.data ?? null);
    } catch (err) {
      console.error('Erro ao carregar dashboard:', err);
      setFetchError(true);
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
      ) : fetchError ? (
        <div className={styles.loader}>
          <p style={{ color: 'var(--error-color)' }}>Não foi possível carregar os dados. Verifique se você está autenticado.</p>
          <button onClick={fetchDashboard} style={{ marginTop: '1rem', padding: '0.5rem 1.5rem', background: 'var(--primary-color)', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer' }}>
            Tentar novamente
          </button>
        </div>
      ) : (
        <>
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
              <div className={styles.value}>{formatCurrency(data?.totalExpenses || 0)}</div>
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

          <div className={styles.reportsGrid}>
            <section className={`${styles.reportSection} glass-panel`}>
              <h2>Totais por Pessoa</h2>
              <table className={styles.reportTable}>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th className="text-right">Receitas</th>
                    <th className="text-right">Despesas</th>
                    <th className="text-right">Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  {data?.totalsByPerson.map(p => (
                    <tr key={p.name}>
                      <td>{p.name}</td>
                      <td className="text-right text-success">{formatCurrency(p.totalRevenue)}</td>
                      <td className="text-right text-danger">{formatCurrency(p.totalExpenses)}</td>
                      <td className={`text-right font-bold ${p.balance < 0 ? 'text-danger' : 'text-success'}`}>
                        {formatCurrency(p.balance)}
                      </td>
                    </tr>
                  ))}
                  {(!data?.totalsByPerson || data.totalsByPerson.length === 0) && (
                    <tr><td colSpan={4} className="text-center">Nenhum dado disponível.</td></tr>
                  )}
                </tbody>
              </table>
            </section>

            <section className={`${styles.reportSection} glass-panel`}>
              <h2>Totais por Categoria</h2>
              <table className={styles.reportTable}>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th className="text-right">Receitas</th>
                    <th className="text-right">Despesas</th>
                    <th className="text-right">Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  {data?.totalsByCategory.map((c, idx) => (
                    <tr key={c.description || idx}>
                      <td>{c.description}</td>
                      <td className="text-right text-success">{formatCurrency(c.totalRevenue)}</td>
                      <td className="text-right text-danger">{formatCurrency(c.totalExpenses)}</td>
                      <td className={`text-right font-bold ${c.balance < 0 ? 'text-danger' : 'text-success'}`}>
                        {formatCurrency(c.balance)}
                      </td>
                    </tr>
                  ))}
                  {(!data?.totalsByCategory || data.totalsByCategory.length === 0) && (
                    <tr><td colSpan={4} className="text-center">Nenhum dado disponível.</td></tr>
                  )}
                </tbody>
              </table>
            </section>
          </div>
        </>
      )}
    </div>
  );
};
