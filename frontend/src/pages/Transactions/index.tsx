import React, { useEffect, useMemo, useState } from 'react';
import api from '../../services/api';
import { Modal } from '../../components/Modal';
import { Plus, Pencil, Trash2, Loader2, ArrowUpCircle, ArrowDownCircle, User, X } from 'lucide-react';
import styles from './styles.module.css';

interface Category { id: string; description: string; purpose: number | string; }
interface Person { id: string; name: string; birthDate: string; }

interface Transaction {
  id: string;
  description: string;
  amount: number;
  date: string;
  type: number | string; //0 = Revenue, 1 = Expense
  categoryId: string;
  categoryDescription?: string;
  personId: string;
  personName?: string;
}

export const Transactions: React.FC = () => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [persons, setPersons] = useState<Person[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTransaction, setEditingTransaction] = useState<Transaction | null>(null);
  const [selectedPersonId, setSelectedPersonId] = useState<string>('');

  const [form, setForm] = useState({
    description: '',
    amount: 0,
    date: new Date().toISOString().split('T')[0],
    type: 0,
    categoryId: '',
    personId: ''
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      const [transRes, catRes, perRes] = await Promise.all([
        api.get('/transactions'),
        api.get('/categories'),
        api.get('/persons')
      ]);

      const transData = transRes.data.data;
      setTransactions(Array.isArray(transData?.items) ? transData.items : []);
      setCategories(catRes.data.data || []);
      setPersons(perRes.data.data || []);
    } catch (err) {
      console.error('Erro ao buscar dados:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  //Transações filtradas pela pessoa selecionada
  const filteredTransactions = useMemo(() => {
    if (!selectedPersonId) return transactions;
    return transactions.filter(t => t.personId === selectedPersonId);
  }, [transactions, selectedPersonId]);

  //Totais calculados a partir das transações filtradas
  const totals = useMemo(() => {
    const revenue = filteredTransactions.filter(t => t.type === 0 || t.type === 'Revenue').reduce((sum, t) => sum + t.amount, 0);
    const expenses = filteredTransactions.filter(t => t.type === 1 || t.type === 'Expense').reduce((sum, t) => sum + t.amount, 0);
    return { revenue, expenses, balance: revenue - expenses };
  }, [filteredTransactions]);

  const handleOpenModal = (transaction?: Transaction) => {
    if (transaction) {
      setEditingTransaction(transaction);
      setForm({
        description: transaction.description,
        amount: transaction.amount,
        date: transaction.date.split('T')[0],
        type: transaction.type === 'Revenue' ? 0 : (transaction.type === 'Expense' ? 1 : Number(transaction.type)),
        categoryId: transaction.categoryId,
        personId: transaction.personId
      });
    } else {
      setEditingTransaction(null);
      setForm({
        description: '',
        amount: 0,
        date: new Date().toISOString().split('T')[0],
        type: 0,
        categoryId: categories[0]?.id || '',
        personId: persons[0]?.id || ''
      });
    }
    setError(null);
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (form.amount <= 0) {
      setError('O valor deve ser maior que zero.');
      return;
    }

    setSubmitting(true);
    const selectedCategory = categories.find(c => c.id === form.categoryId);

    //Validação de Finalidade da Categoria
    if (selectedCategory) {
      const isBoth = selectedCategory.purpose === 2 || selectedCategory.purpose === 'Both';
      const isRevenue = selectedCategory.purpose === 0 || selectedCategory.purpose === 'Revenue';
      const isExpense = selectedCategory.purpose === 1 || selectedCategory.purpose === 'Expense';

      const isCompatible = isBoth ||
        (form.type === 0 && isRevenue) ||
        (form.type === 1 && isExpense);

      if (!isCompatible) {
        const purposeText = isRevenue ? 'apenas receitas' : 'apenas despesas';
        setError(`A categoria selecionada é permitida apenas para ${purposeText}.`);
        setSubmitting(false);
        return;
      }
    }

    try {
      if (editingTransaction) {
        await api.put(`/transactions/${editingTransaction.id}`, form);
      } else {
        await api.post('/transactions', form);
      }
      setIsModalOpen(false);
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao salvar lançamento.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Excluir este lançamento?')) return;
    try {
      await api.delete(`/transactions/${id}`);
      fetchData();
    } catch (err) {
      alert('Erro ao excluir lançamento.');
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

  const formatDate = (dateString: string) =>
    new Date(dateString).toLocaleDateString('pt-BR');

  const selectedPersonName = persons.find(p => p.id === selectedPersonId)?.name;

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>Transações</h1>
          <p>Registre e acompanhe todas as movimentações financeiras.</p>
        </div>
        <button className={styles.addBtn} onClick={() => handleOpenModal()}>
          <Plus size={20} />
          <span>Novo Lançamento</span>
        </button>
      </div>

      {/* Filtro por Pessoa */}
      <div className={`${styles.filterBar} glass-panel`}>
        <div className={styles.filterGroup}>
          <User size={16} />
          <label>Filtrar por Pessoa:</label>
          <select
            value={selectedPersonId}
            onChange={e => setSelectedPersonId(e.target.value)}
            className={styles.filterSelect}
          >
            <option value="">Todas as Pessoas</option>
            {persons.map(p => (
              <option key={p.id} value={p.id}>{p.name}</option>
            ))}
          </select>
          {selectedPersonId && (
            <button className={styles.clearBtn} onClick={() => setSelectedPersonId('')} title="Limpar filtro">
              <X size={14} />
            </button>
          )}
        </div>

        {/* Painel de Totais Dinâmico */}
        <div className={styles.totalsBar}>
          <span className={styles.totalLabel}>
            {selectedPersonName ? `Totais de ${selectedPersonName}` : 'Totais Gerais'}:
          </span>
          <span className={styles.totalRevenue}>
            ↑ {formatCurrency(totals.revenue)}
          </span>
          <span className={styles.totalExpense}>
            ↓ {formatCurrency(totals.expenses)}
          </span>
          <span className={totals.balance >= 0 ? styles.totalBalancePos : styles.totalBalanceNeg}>
            = {formatCurrency(totals.balance)}
          </span>
        </div>
      </div>

      {loading ? (
        <div className={styles.loader}><Loader2 className="spinning" size={40} /></div>
      ) : (
        <div className={`${styles.tableWrapper} glass-panel`}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th>Data</th>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Pessoa</th>
                <th>Valor</th>
                <th style={{ width: '100px' }}>Ações</th>
              </tr>
            </thead>
            <tbody>
              {filteredTransactions.length === 0 ? (
                <tr>
                  <td colSpan={6} style={{ textAlign: 'center', padding: '2rem' }}>
                    {selectedPersonId ? `Nenhum lançamento para ${selectedPersonName}.` : 'Nenhum lançamento encontrado.'}
                  </td>
                </tr>
              ) : (
                filteredTransactions.map(t => (
                  <tr key={t.id}>
                    <td>{formatDate(t.date)}</td>
                    <td>{t.description}</td>
                    <td><span className={styles.badge}>{t.categoryDescription}</span></td>
                    <td>{t.personName}</td>
                    <td className={t.type === 0 || t.type === 'Revenue' ? styles.income : styles.expense}>
                      <div className={styles.amountCell}>
                        {t.type === 0 || t.type === 'Revenue' ? <ArrowUpCircle size={14} /> : <ArrowDownCircle size={14} />}
                        {formatCurrency(t.amount)}
                      </div>
                    </td>
                    <td className={styles.actions}>
                      <button onClick={() => handleOpenModal(t)} className={styles.editBtn}><Pencil size={18} /></button>
                      <button onClick={() => handleDelete(t.id)} className={styles.deleteBtn}><Trash2 size={18} /></button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title={editingTransaction ? 'Editar Lançamento' : 'Novo Lançamento'}>
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className="input-group">
            <label>Descrição</label>
            <input type="text" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="Ex: Pagamento Fornecedor X" required />
          </div>

          <div className={styles.row}>
            <div className="input-group">
              <label>Valor (R$)</label>
              <input type="number" step="0.01" value={form.amount} onChange={e => setForm({ ...form, amount: parseFloat(e.target.value) })} required />
            </div>
            <div className="input-group">
              <label>Data</label>
              <input type="date" value={form.date} onChange={e => setForm({ ...form, date: e.target.value })} required />
            </div>
          </div>

          <div className={styles.row}>
            <div className="input-group">
              <label>Tipo</label>
              <select value={form.type} onChange={e => setForm({ ...form, type: parseInt(e.target.value) })}>
                <option value={0}>Entrada (Receita)</option>
                <option value={1}>Saída (Despesa)</option>
              </select>
            </div>
            <div className="input-group">
              <label>Categoria</label>
              <select value={form.categoryId} onChange={e => setForm({ ...form, categoryId: e.target.value })} required>
                <option value="">Selecione...</option>
                {categories
                  .filter(c => {
                    const purpose = c.purpose;
                    const isBoth = purpose === 2 || purpose === 'Both';
                    const isRevenue = purpose === 0 || purpose === 'Revenue';
                    const isExpense = purpose === 1 || purpose === 'Expense';

                    if (isBoth) return true;
                    if (form.type === 0) return isRevenue;
                    if (form.type === 1) return isExpense;
                    return false;
                  })
                  .map(c => <option key={c.id} value={c.id}>{c.description}</option>)}
              </select>
            </div>
          </div>

          <div className="input-group">
            <label>Pessoa Relacionada</label>
            <select value={form.personId} onChange={e => setForm({ ...form, personId: e.target.value })} required>
              <option value="">Selecione...</option>
              {persons.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </div>

          {error && <p className={styles.error}>{error}</p>}

          <div className={styles.modalActions}>
            <button type="button" onClick={() => setIsModalOpen(false)} className={styles.cancelBtn}>Cancelar</button>
            <button type="submit" className={styles.submitBtn} disabled={submitting}>{submitting ? 'Salvando...' : 'Salvar'}</button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
