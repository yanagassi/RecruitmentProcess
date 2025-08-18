import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { employeeService } from '../services/api';

const EmployeeForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;
  
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    docNumber: '',
    birthDate: '',
    managerName: '',
    phones: [{ number: '' }],
    password: '',
    confirmPassword: '',
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [managers, setManagers] = useState([]);
  
  useEffect(() => {
  
    const fetchManagers = async () => {
      try {
        const data = await employeeService.getAll();
        setManagers(data);
      } catch (err) {
        console.error('Erro ao carregar gerentes:', err);
      }
    };
    
    fetchManagers();
    
    if (id) {
      const fetchEmployee = async () => {
        try {
          setLoading(true);
          const data = await employeeService.getById(id);
          
          // Formatar a data para o formato esperado pelo input date
          const formattedData = {
            ...data,
            birthDate: data.birthDate ? new Date(data.birthDate).toISOString().split('T')[0] : '',
            // Garantir que phones seja um array com pelo menos um item
            phones: data.phones && data.phones.length > 0 ? data.phones : [{ number: '' }],
            password: '',
            confirmPassword: '',
          };
          
          setFormData(formattedData);
        } catch (err) {
          setError('Erro ao carregar dados do funcionário. Por favor, tente novamente.');
          console.error(err);
        } finally {
          setLoading(false);
        }
      };
      
      fetchEmployee();
    }
  }, [id, isEditMode]);
  
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };
  
  const handlePhoneChange = (index, value) => {
    const updatedPhones = [...formData.phones];
    updatedPhones[index] = { number: value };
    setFormData({ ...formData, phones: updatedPhones });
  };
  
  const addPhoneField = () => {
    setFormData({
      ...formData,
      phones: [...formData.phones, { number: '' }],
    });
  };
  
  const removePhoneField = (index) => {
    if (formData.phones.length > 1) {
      const updatedPhones = formData.phones.filter((_, i) => i !== index);
      setFormData({ ...formData, phones: updatedPhones });
    }
  };
  
  const validateAge = (birthDate) => {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age >= 18;
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    if (!isEditMode && formData.password !== formData.confirmPassword) {
      setError('As senhas não coincidem');
      return;
    }
    
    if (!validateAge(formData.birthDate)) {
      setError('O funcionário deve ter pelo menos 18 anos');
      return;
    }
    
    setLoading(true);
    
    try {
      // Remover campos desnecessários antes de enviar para a API
      const { confirmPassword, ...employeeData } = formData;
      
      // Remover senha se estiver vazia no modo de edição
      const dataToSubmit = isEditMode && !employeeData.password 
        ? { ...employeeData, password: undefined } 
        : employeeData;
      
      if (isEditMode) {
        await employeeService.update(id, dataToSubmit);
      } else {
        await employeeService.create(dataToSubmit);
      }
      
      navigate('/');
    } catch (err) {
      setError(err.response?.data?.message || 'Erro ao salvar funcionário. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };
  
  if (loading && isEditMode) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
      </div>
    );
  }
  
  return (
    <div className="bg-white p-8 rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold text-center text-gray-800 mb-8">
        {isEditMode ? 'Editar Funcionário' : 'Adicionar Funcionário'}
      </h2>
      
      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4" role="alert">
          <span className="block sm:inline">{error}</span>
        </div>
      )}
      
      <form onSubmit={handleSubmit}>
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div>
            <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="firstName">
              Nome *
            </label>
            <input
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              id="firstName"
              type="text"
              name="firstName"
              placeholder="Nome"
              value={formData.firstName}
              onChange={handleChange}
              required
            />
          </div>
          
          <div>
            <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="lastName">
              Sobrenome *
            </label>
            <input
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              id="lastName"
              type="text"
              name="lastName"
              placeholder="Sobrenome"
              value={formData.lastName}
              onChange={handleChange}
              required
            />
          </div>
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="email">
            Email *
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="email"
            type="email"
            name="email"
            placeholder="seu@email.com"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="docNumber">
            CPF/Documento *
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="docNumber"
            type="text"
            name="docNumber"
            placeholder="000.000.000-00"
            value={formData.docNumber}
            onChange={handleChange}
            required
            disabled={isEditMode} // Não permitir edição do documento em modo de edição
          />
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="birthDate">
            Data de Nascimento *
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="birthDate"
            type="date"
            name="birthDate"
            value={formData.birthDate}
            onChange={handleChange}
            required
          />
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="managerName">
            Gerente
          </label>
          <select
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="managerName"
            name="managerName"
            value={formData.managerName || ''}
            onChange={handleChange}
          >
            <option value="">Selecione um gerente (opcional)</option>
            {managers.map((manager) => (
              <option 
                key={manager.id} 
                value={`${manager.firstName} ${manager.lastName}`}
              >
                {manager.firstName} {manager.lastName}
              </option>
            ))}
          </select>
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2">
            Telefones
          </label>
          {formData.phones.map((phone, index) => (
            <div key={index} className="flex mb-2">
              <input
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                type="text"
                placeholder="(00) 00000-0000"
                value={phone.number}
                onChange={(e) => handlePhoneChange(index, e.target.value)}
              />
              <button
                type="button"
                onClick={() => removePhoneField(index)}
                className="ml-2 bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                disabled={formData.phones.length <= 1}
              >
                -
              </button>
              {index === formData.phones.length - 1 && (
                <button
                  type="button"
                  onClick={addPhoneField}
                  className="ml-2 bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                >
                  +
                </button>
              )}
            </div>
          ))}
        </div>
        
        {!isEditMode && (
          <>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="password">
                Senha *
              </label>
              <input
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                id="password"
                type="password"
                name="password"
                placeholder="********"
                value={formData.password}
                onChange={handleChange}
                required={!isEditMode}
                minLength="8"
              />
            </div>
            
            <div className="mb-6">
              <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="confirmPassword">
                Confirmar Senha *
              </label>
              <input
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                id="confirmPassword"
                type="password"
                name="confirmPassword"
                placeholder="********"
                value={formData.confirmPassword}
                onChange={handleChange}
                required={!isEditMode}
                minLength="8"
              />
            </div>
          </>
        )}
        
        {isEditMode && (
          <>
            <div className="mb-4">
              <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="password">
                Nova Senha (deixe em branco para manter a atual)
              </label>
              <input
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                id="password"
                type="password"
                name="password"
                placeholder="********"
                value={formData.password}
                onChange={handleChange}
                minLength="8"
              />
            </div>
            
            <div className="mb-6">
              <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="confirmPassword">
                Confirmar Nova Senha
              </label>
              <input
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                id="confirmPassword"
                type="password"
                name="confirmPassword"
                placeholder="********"
                value={formData.confirmPassword}
                onChange={handleChange}
                minLength="8"
              />
            </div>
          </>
        )}
        
        <div className="flex items-center justify-between">
          <button
            type="button"
            onClick={() => navigate('/')}
            className="bg-gray-500 hover:bg-gray-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
          >
            Cancelar
          </button>
          <button
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            type="submit"
            disabled={loading}
          >
            {loading ? 'Salvando...' : 'Salvar'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default EmployeeForm;