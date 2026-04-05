import React, { useEffect, useState } from 'react';
import api from '../../services/api';
import { Modal } from '../../components/Modal';
import { Plus, Pencil, Trash2, Loader2, ArrowUpCircle, ArrowDownCircle } from 'lucide-react';
import styles from './styles.module.css';

interface Category { id: string; name: string; }
interface Person { id: string; name: string; }

interface Transaction {
  id: string;
  description: string;
  amount: number;
  date: string;
  type: number; //0 = Revenue, 1 = Expense
  categoryId: string;
  categoryName?: string;
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
      setTransactions(transRes.data.data || []);
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

  const handleOpenModal = (transaction?: Transaction) => {
    if (transaction) {
      setEditingTransaction(transaction);
      setForm({
        description: transaction.description,
        amount: transaction.amount,
        date: transaction.date.split('T')[0],
        type: transaction.type,
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

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  const formatDate = (dateString: string) => {
      return new Date(dateString).toLocaleDateString('pt-BR');
  }

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
              {transactions.length === 0 ? (
                <tr><td colSpan={6} style={{ textAlign: 'center', padding: '2rem' }}>Nenhum lançamento encontrado.</td></tr>
              ) : (
                transactions.map(t => (
                  <tr key={t.id}>
                    <td>{formatDate(t.date)}</td>
                    <td>{t.description}</td>
                    <td><span className={styles.badge}>{t.categoryName}</span></td>
                    <td>{t.personName}</td>
                    <td className={t.type === 0 ? styles.income : styles.expense}>
                        <div className={styles.amountCell}>
                            {t.type === 0 ? <ArrowUpCircle size={14} /> : <ArrowDownCircle size={14} />}
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
            <input type="text" value={form.description} onChange={e => setForm({...form, description: e.target.value})} placeholder="Ex: Pagamento Fornecedor X" required />
          </div>

          <div className={styles.row}>
            <div className="input-group">
                <label>Valor (R$)</label>
                <input type="number" step="0.01" value={form.amount} onChange={e => setForm({...form, amount: parseFloat(e.target.value)})} required />
            </div>
            <div className="input-group">
                <label>Data</label>
                <input type="date" value={form.date} onChange={e => setForm({...form, date: e.target.value})} required />
            </div>
          </div>

          <div className={styles.row}>
            <div className="input-group">
                <label>Tipo</label>
                <select value={form.type} onChange={e => setForm({...form, type: parseInt(e.target.value)})}>
                    <option value={0}>Entrada (Receita)</option>
                    <option value={1}>Saída (Despesa)</option>
                </select>
            </div>
            <div className="input-group">
                <label>Categoria</label>
                <select value={form.categoryId} onChange={e => setForm({...form, categoryId: e.target.value})} required>
                    <option value="">Selecione...</option>
                    {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
            </div>
          </div>

          <div className="input-group">
            <label>Pessoa Relacionada</label>
            <select value={form.personId} onChange={e => setForm({...form, personId: e.target.value})} required>
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
