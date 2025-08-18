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
    position: '',
    department: '',
    salary: '',
    hireDate: '',
    managerId: '',
    managerName: '',
    permissionLevel: 'Employee',
    phones: [{ phoneNumber: '', phoneType: 'Mobile', isPrimary: true }],
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
    
    if (isEditMode && id) {
      const fetchEmployee = async () => {
        try {
          setLoading(true);
          const data = await employeeService.getById(id);
          // Calcular birthDate a partir da idade e hireDate
          const calculateBirthDate = (age, hireDate) => {
            if (!age || !hireDate) return '';
            const hire = new Date(hireDate);
            const birthYear = hire.getFullYear() - age;
            return `${birthYear}-01-01`; // Aproximação
          };

          // Formatar os dados para o formato esperado pelo frontend
          const formattedData = {
            firstName: data.firstName || '',
            lastName: data.lastName || '',
            email: data.email || '',
            docNumber: data.docNumber || '',
            birthDate: calculateBirthDate(data.age, data.hireDate),
            position: data.position || '',
            department: data.department || '',
            salary: data.salary ? data.salary.toString() : '',
            hireDate: data.hireDate ? new Date(data.hireDate).toISOString().split('T')[0] : '',
            managerId: data.managerId || '',
            managerName: data.managerName || '',
            permissionLevel: data.permissionLevel === 1 ? 'Employee' : data.permissionLevel === 2 ? 'Leader' : data.permissionLevel === 3 ? 'Director' : 'Employee',
            // Garantir que phones seja um array com pelo menos um item
            phones: data.phones && data.phones.length > 0 
              ? data.phones.map(phone => ({
                  phoneNumber: phone.phoneNumber || '',
                  phoneType: phone.phoneType || 'Mobile',
                  isPrimary: phone.isPrimary || false
                }))
              : [{ phoneNumber: '', phoneType: 'Mobile', isPrimary: true }],
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
  
  const handlePhoneChange = (index, field, value) => {
    const updatedPhones = [...formData.phones];
    updatedPhones[index] = { ...updatedPhones[index], [field]: value };
    setFormData({ ...formData, phones: updatedPhones });
  };
  
  const addPhoneField = () => {
    setFormData({
      ...formData,
      phones: [...formData.phones, { phoneNumber: '', phoneType: 'Mobile', isPrimary: false }],
    });
  };
  
  const removePhoneField = (index) => {
    if (formData.phones.length > 1) {
      const updatedPhones = formData.phones.filter((_, i) => i !== index);
      setFormData({ ...formData, phones: updatedPhones });
    }
  };
  
  const validateAge = (birthDate) => {
    if (!birthDate) return false;
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age >= 16; // Mudando para 16 anos conforme validação do backend
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    if (!isEditMode && formData.password !== formData.confirmPassword) {
      setError('As senhas não coincidem');
      return;
    }
    
    if (!validateAge(formData.birthDate)) {
      setError('O funcionário deve ter pelo menos 16 anos');
      return;
    }
    
    setLoading(true);
    
    try {
      // Calcular idade a partir da data de nascimento
      const calculateAge = (birthDate) => {
        const today = new Date();
        const birth = new Date(birthDate);
        let age = today.getFullYear() - birth.getFullYear();
        const monthDiff = today.getMonth() - birth.getMonth();
        
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
          age--;
        }
        
        return age;
      };

      // Encontrar o managerId baseado no managerName
      const findManagerId = (managerName) => {
        if (!managerName) return null;
        const manager = managers.find(m => `${m.firstName} ${m.lastName}` === managerName);
        return manager ? manager.id : null;
      };

      // Converter dados do frontend para o formato do backend
      const { confirmPassword, birthDate, managerName, ...baseData } = formData;
      
      const calculatedAge = calculateAge(birthDate);
      
      const backendData = {
        ...baseData,
        age: calculatedAge >= 16 ? calculatedAge : 18, // Garantir idade mínima
        salary: parseFloat(formData.salary) || 0,
        hireDate: formData.hireDate ? new Date(formData.hireDate).toISOString() : new Date().toISOString(),
        managerId: findManagerId(managerName),
        phones: formData.phones
          .filter(phone => phone.phoneNumber.trim() !== '')
          .map(phone => ({
            phoneNumber: phone.phoneNumber,
            phoneType: phone.phoneType || 'Mobile',
            isPrimary: phone.isPrimary || false
          }))
      };
      
      // Remover senha se estiver vazia no modo de edição
      const dataToSubmit = isEditMode && !backendData.password 
        ? { ...backendData, password: undefined } 
        : backendData;
      
      if (isEditMode) {
        console.log('Enviando dados para update:', dataToSubmit);
        const result = await employeeService.update(id, dataToSubmit);
        console.log('Resultado do update:', result);
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
        
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div>
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
          
          <div>
            <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="hireDate">
              Data de Contratação *
            </label>
            <input
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              id="hireDate"
              type="date"
              name="hireDate"
              value={formData.hireDate}
              onChange={handleChange}
              required
            />
          </div>
        </div>
        
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div>
            <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="position">
              Cargo *
            </label>
            <input
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              id="position"
              type="text"
              name="position"
              placeholder="Ex: Desenvolvedor, Analista"
              value={formData.position}
              onChange={handleChange}
              required
            />
          </div>
          
          <div>
            <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="department">
              Departamento
            </label>
            <input
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              id="department"
              type="text"
              name="department"
              placeholder="Ex: TI, RH, Vendas"
              value={formData.department}
              onChange={handleChange}
            />
          </div>
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="salary">
            Salário
          </label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="salary"
            type="number"
            name="salary"
            placeholder="0.00"
            step="0.01"
            min="0"
            value={formData.salary}
            onChange={handleChange}
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
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="permissionLevel">
            Nível de Permissão *
          </label>
          <select
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="permissionLevel"
            name="permissionLevel"
            value={formData.permissionLevel}
            onChange={handleChange}
            required
          >
            <option value="Employee">Funcionário</option>
            <option value="Leader">Líder</option>
            <option value="Director">Diretor</option>
          </select>
        </div>
        
        <div className="mb-4">
          <label className="block text-gray-700 text-sm font-bold mb-2">
            Telefones
          </label>
          {formData.phones.map((phone, index) => (
            <div key={index} className="border rounded p-3 mb-3 bg-gray-50">
              <div className="grid grid-cols-3 gap-2 mb-2">
                <div>
                  <label className="block text-gray-600 text-xs font-bold mb-1">
                    Número *
                  </label>
                  <input
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    type="text"
                    placeholder="(00) 00000-0000"
                    value={phone.phoneNumber}
                    onChange={(e) => handlePhoneChange(index, 'phoneNumber', e.target.value)}
                    required
                  />
                </div>
                <div>
                  <label className="block text-gray-600 text-xs font-bold mb-1">
                    Tipo
                  </label>
                  <select
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    value={phone.phoneType}
                    onChange={(e) => handlePhoneChange(index, 'phoneType', e.target.value)}
                  >
                    <option value="Mobile">Celular</option>
                    <option value="Home">Residencial</option>
                    <option value="Work">Comercial</option>
                  </select>
                </div>
                <div className="flex items-center">
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      className="mr-2"
                      checked={phone.isPrimary}
                      onChange={(e) => handlePhoneChange(index, 'isPrimary', e.target.checked)}
                    />
                    <span className="text-gray-600 text-xs font-bold">Principal</span>
                  </label>
                </div>
              </div>
              <div className="flex justify-end">
                <button
                  type="button"
                  onClick={() => removePhoneField(index)}
                  className="bg-red-500 hover:bg-red-600 text-white font-bold py-1 px-3 rounded text-sm focus:outline-none focus:shadow-outline mr-2"
                  disabled={formData.phones.length <= 1}
                >
                  Remover
                </button>
                {index === formData.phones.length - 1 && (
                  <button
                    type="button"
                    onClick={addPhoneField}
                    className="bg-green-500 hover:bg-green-600 text-white font-bold py-1 px-3 rounded text-sm focus:outline-none focus:shadow-outline"
                  >
                    Adicionar
                  </button>
                )}
              </div>
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