import React, { useEffect, useState } from 'react';
import api from '../../services/api';
import { Modal } from '../../components/Modal';
import { Plus, Pencil, Trash2, Loader2 } from 'lucide-react';
import styles from './styles.module.css';

interface Category {
  id: string;
  name: string;
}

export const Categories: React.FC = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [name, setName] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCategories = async () => {
    try {
      const response = await api.get('/categories');
      //A API retorna o Result<T>, o dado real está em .data
      setCategories(response.data.data || []);
    } catch (err) {
      console.error('Erro ao buscar categorias:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleOpenModal = (category?: Category) => {
    if (category) {
      setEditingCategory(category);
      setName(category.name);
    } else {
      setEditingCategory(null);
      setName('');
    }
    setError(null);
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;

    setSubmitting(true);
    setError(null);

    try {
      if (editingCategory) {
        await api.put(`/categories/${editingCategory.id}`, { name });
      } else {
        await api.post('/categories', { name });
      }
      setIsModalOpen(false);
      fetchCategories();
    } catch (err: any) {
      const msg = err.response?.data?.message || 'Erro ao salvar categoria.';
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja excluir esta categoria?')) return;

    try {
      await api.delete(`/categories/${id}`);
      fetchCategories();
    } catch (err) {
      alert('Erro ao excluir categoria. Ela pode estar vinculada a transações.');
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>Categorias</h1>
          <p>Gerencie as categorias para seus lançamentos financeiros.</p>
        </div>
        <button className={styles.addBtn} onClick={() => handleOpenModal()}>
          <Plus size={20} />
          <span>Nova Categoria</span>
        </button>
      </div>

      {loading ? (
        <div className={styles.loader}>
          <Loader2 className="spinning" size={40} />
        </div>
      ) : (
        <div className={`${styles.tableWrapper} glass-panel`}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th>Nome</th>
                <th style={{ width: '120px' }}>Ações</th>
              </tr>
            </thead>
            <tbody>
              {categories.length === 0 ? (
                <tr>
                  <td colSpan={2} style={{ textAlign: 'center', padding: '2rem' }}>
                    Nenhuma categoria encontrada.
                  </td>
                </tr>
              ) : (
                categories.map(category => (
                  <tr key={category.id}>
                    <td>{category.name}</td>
                    <td className={styles.actions}>
                      <button onClick={() => handleOpenModal(category)} className={styles.editBtn}>
                        <Pencil size={18} />
                      </button>
                      <button onClick={() => handleDelete(category.id)} className={styles.deleteBtn}>
                        <Trash2 size={18} />
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      <Modal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        title={editingCategory ? 'Editar Categoria' : 'Nova Categoria'}
      >
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className="input-group">
            <label>Nome da Categoria</label>
            <input 
              type="text" 
              value={name} 
              onChange={e => setName(e.target.value)} 
              placeholder="Ex: Alimentação, Salário..."
              autoFocus
            />
          </div>
          
          {error && <p className={styles.error}>{error}</p>}

          <div className={styles.modalActions}>
            <button type="button" onClick={() => setIsModalOpen(false)} className={styles.cancelBtn}>
              Cancelar
            </button>
            <button type="submit" className={styles.submitBtn} disabled={submitting}>
              {submitting ? 'Salvando...' : 'Salvar'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
