import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

const api = axios.create({
  baseURL: API_URL,
});

// Interceptor para adicionar o token de autenticação em todas as requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor para tratar erros de autenticação
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    return response.data;
  },
  register: async (userData) => {
    const response = await api.post('/auth/register', userData);
    return response.data;
  },
};

export const employeeService = {
  getAll: async () => {
    const response = await api.get('/employees');
    return response.data.employees || [];
  },
  getById: async (id) => {
    const response = await api.get(`/employees/${id}`);
    return response.data.employee;
  },
  create: async (employeeData) => {
    console.log('API create - dados recebidos:', employeeData);
    
    // Converter permissionLevel de string para número
    const permissionLevelMap = {
      'Employee': 1,
      'Leader': 2,
      'Director': 3
    };

    // Preparar dados no formato esperado pelo backend
    const createEmployeeDto = {
      firstName: employeeData.firstName || '',
      lastName: employeeData.lastName || '',
      email: employeeData.email || '',
      docNumber: employeeData.docNumber || '',
      age: employeeData.age ? parseInt(employeeData.age) : 18,
      position: employeeData.position || '',
      department: employeeData.department || '',
      salary: employeeData.salary ? parseFloat(employeeData.salary) : 0,
      hireDate: employeeData.hireDate ? new Date(employeeData.hireDate).toISOString() : new Date().toISOString(),
      managerId: employeeData.managerId ? parseInt(employeeData.managerId) : null,
      permissionLevel: permissionLevelMap[employeeData.permissionLevel] ?? 1,
      phones: employeeData.phones?.map(phone => ({
        phoneNumber: phone.phoneNumber,
        phoneType: phone.phoneType,
        isPrimary: phone.isPrimary
      })) || []
    };

    console.log('API create - dados formatados:', createEmployeeDto);
    console.log('API create - enviando para:', '/employees');

    const response = await api.post('/employees', { createEmployeeDto });
    console.log('API create - resposta:', response.data);
    return response.data;
  },
  update: async (id, employeeData) => {
    console.log('API update - dados recebidos:', employeeData);
    
    // Converter permissionLevel de string para número
    const permissionLevelMap = {
      'Employee': 1,
      'Leader': 2,
      'Director': 3
    };

    // Preparar dados no formato esperado pelo backend
    const updateEmployeeDto = {
      firstName: employeeData.firstName,
      lastName: employeeData.lastName,
      email: employeeData.email,
      docNumber: employeeData.docNumber,
      age: employeeData.age ? parseInt(employeeData.age) : null,
      position: employeeData.position,
      department: employeeData.department,
      salary: employeeData.salary ? parseFloat(employeeData.salary) : null,
      hireDate: employeeData.hireDate ? new Date(employeeData.hireDate).toISOString() : null,
      managerId: employeeData.managerId ? parseInt(employeeData.managerId) : null,
      permissionLevel: permissionLevelMap[employeeData.permissionLevel] ?? 1,
      phones: employeeData.phones?.map(phone => ({
        phoneNumber: phone.phoneNumber,
        phoneType: phone.phoneType,
        isPrimary: phone.isPrimary
      })) || []
    };

    console.log('API update - dados formatados:', updateEmployeeDto);
    console.log('API update - enviando para:', `/employees/${id}`);

    const response = await api.put(`/employees/${id}`, { updateEmployeeDto });
    console.log('API update - resposta:', response.data);
    return response.data;
  },
  delete: async (id) => {
    const response = await api.delete(`/employees/${id}`);
    return response.data;
  },
};

export default api;