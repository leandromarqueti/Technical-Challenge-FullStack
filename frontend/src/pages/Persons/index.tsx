import React, { useEffect, useState } from 'react';
import api from '../../services/api';
import { Modal } from '../../components/Modal';
import { Plus, Pencil, Trash2, Loader2, User } from 'lucide-react';
import styles from './styles.module.css';

interface Person {
  id: string;
  name: string;
  birthDate: string;
  document: string;
}

export const Persons: React.FC = () => {
  const [persons, setPersons] = useState<Person[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingPerson, setEditingPerson] = useState<Person | null>(null);
  
  //estado do form
  const [form, setForm] = useState({
    name: '',
    birthDate: '',
    document: ''
  });
  
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchPersons = async () => {
    try {
      const response = await api.get('/persons');
      const data = response.data;
      console.log('Dados recebidos da API /persons:', data);
      
      //Tentar diferentes formatos de resposta
      const list = data?.data || data?.Data || (Array.isArray(data) ? data : []);
      setPersons(list);
    } catch (err) {
      console.error('Erro ao buscar pessoas:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPersons();
  }, []);

  const handleOpenModal = (person?: Person) => {
    if (person) {
      setEditingPerson(person);
      setForm({
        name: person.name,
        birthDate: person.birthDate.split('T')[0],
        document: person.document
      });
    } else {
      setEditingPerson(null);
      setForm({ name: '', birthDate: '', document: '' });
    }
    setError(null);
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      if (editingPerson) {
        await api.put(`/persons/${editingPerson.id}`, form);
      } else {
        await api.post('/persons', form);
      }
      
      setIsModalOpen(false);
      setEditingPerson(null);
      fetchPersons();
    } catch (err: any) {
      const data = err.response?.data;
      //tenta entender o erro retornado pela api
      const msg = data?.message || data?.Message || err.message || 'Erro inesperado ao salvar pessoa.';
      setError(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Tem certeza que deseja excluir esta pessoa?')) {
      try {
        await api.delete(`/persons/${id}`);
        fetchPersons();
      } catch (err) {
        console.error('Erro ao excluir pessoa:', err);
      }
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const formatDocument = (doc: string) => {
    if (doc.length === 11) {
      return doc.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    }
    if (doc.length === 14) {
      return doc.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }
    return doc;
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>Pessoas</h1>
          <p>Cadastre clientes, fornecedores e parceiros de negócio.</p>
        </div>
        <button className={styles.addBtn} onClick={() => handleOpenModal()}>
          <Plus size={20} />
          <span>Nova Pessoa</span>
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
                <th>Documento</th>
                <th>Nascimento</th>
                <th style={{ width: '120px' }}>Ações</th>
              </tr>
            </thead>
            <tbody>
              {persons.length === 0 ? (
                <tr>
                  <td colSpan={4} style={{ textAlign: 'center', padding: '2rem' }}>
                    Nenhuma pessoa encontrada.
                  </td>
                </tr>
              ) : (
                persons.map(person => (
                  <tr key={person.id}>
                    <td className={styles.nameCell}>
                      <User size={16} />
                      {person.name}
                    </td>
                    <td>{formatDocument(person.document)}</td>
                    <td>{formatDate(person.birthDate)}</td>
                    <td className={styles.actions}>
                      <button onClick={() => handleOpenModal(person)} className={styles.editBtn}>
                        <Pencil size={18} />
                      </button>
                      <button onClick={() => handleDelete(person.id)} className={styles.deleteBtn}>
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
        title={editingPerson ? 'Editar Pessoa' : 'Nova Pessoa'}
      >
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className="input-group">
            <label>Nome Completo</label>
            <input 
              type="text" 
              value={form.name} 
              onChange={e => setForm({...form, name: e.target.value})} 
              placeholder="Ex: João Silva"
              required
            />
          </div>

          <div className={styles.row}>
            <div className="input-group">
              <label>Documento (CPF/CNPJ)</label>
              <input 
                type="text" 
                value={form.document} 
                onChange={e => setForm({...form, document: e.target.value.replace(/\D/g, '')})} 
                placeholder="Apenas números"
                maxLength={14}
                required
              />
            </div>
            <div className="input-group">
              <label>Data de Nascimento</label>
              <input 
                type="date" 
                value={form.birthDate} 
                onChange={e => setForm({...form, birthDate: e.target.value})} 
                required
              />
            </div>
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
