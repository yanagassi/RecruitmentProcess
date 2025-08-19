import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { employeeService } from "../services/api";
import FormField from "../components/FormField";
import { useToast } from "../contexts/ToastContext";
import {
  formatCPF,
  formatPhone,
  formatCurrency,
  parseCurrency,
  isValidEmail,
  isValidCPF,
  isValidPhone
} from "../utils/inputMasks";

const EmployeeForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { showSuccess, showError } = useToast();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});
  const [touched, setTouched] = useState({});
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    docNumber: "",
    age: "",
    position: "",
    department: "",
    salary: "",
    hireDate: "",
    managerId: "",
    permissionLevel: "employee",
    phones: [{ phoneNumber: "", phoneType: "Mobile", isPrimary: false }],
  });

  useEffect(() => {
    if (id) {
      fetchEmployee();
    }
  }, [id]);

  const fetchEmployee = async () => {
    try {
      setLoading(true);
      const data = await employeeService.getById(id);
      
      setFormData({
        ...data,
        phones:
          data.phones?.length > 0
            ? data.phones.map((phone) => ({
                phoneNumber: phone.PhoneNumber || phone.phoneNumber || "",
                phoneType: phone.PhoneType || phone.phoneType || "Mobile",
                isPrimary: phone.IsPrimary !== undefined ? phone.IsPrimary : (phone.isPrimary || false),
              }))
            : [{ phoneNumber: "", phoneType: "Mobile", isPrimary: false }],
      });
    } catch (err) {
      const errorMessage = "Erro ao carregar dados do funcionário. Por favor, tente novamente.";
      showError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const validateField = (name, value) => {
    let error = '';
    
    switch (name) {
      case 'firstName':
      case 'lastName':
        if (!value.trim()) {
          error = 'Este campo é obrigatório';
        } else if (value.trim().length < 2) {
          error = 'Deve ter pelo menos 2 caracteres';
        }
        break;
      case 'email':
        if (!value.trim()) {
          error = 'Email é obrigatório';
        } else if (!isValidEmail(value)) {
          error = 'Email inválido';
        }
        break;
      case 'docNumber':
        if (!value.trim()) {
          error = 'CPF é obrigatório';
        } else if (!isValidCPF(value)) {
          error = 'CPF inválido';
        }
        break;
      case 'age':
        const age = parseInt(value);
        if (!value) {
          error = 'Idade é obrigatória';
        } else if (age < 16 || age > 100) {
          error = 'Idade deve estar entre 16 e 100 anos';
        }
        break;
      case 'position':
        if (!value.trim()) {
          error = 'Cargo é obrigatório';
        }
        break;
      case 'department':
        if (!value.trim()) {
          error = 'Departamento é obrigatório';
        }
        break;
      case 'salary':
        const salary = parseCurrency(value);
        if (!value) {
          error = 'Salário é obrigatório';
        } else if (salary <= 0) {
          error = 'Salário deve ser maior que zero';
        }
        break;
    }
    
    return error;
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    let formattedValue = value;
    
    switch (name) {
      case 'docNumber':
        formattedValue = formatCPF(value);
        break;
      case 'salary':
        formattedValue = formatCurrency(value);
        break;
    }
    
    setFormData((prev) => ({
      ...prev,
      [name]: formattedValue,
    }));
    
    if (touched[name]) {
      const error = validateField(name, formattedValue);
      setFieldErrors(prev => ({
        ...prev,
        [name]: error
      }));
    }
  };
  
  const handleBlur = (e) => {
    const { name, value } = e.target;
    
    setTouched(prev => ({
      ...prev,
      [name]: true
    }));
    
    const error = validateField(name, value);
    setFieldErrors(prev => ({
      ...prev,
      [name]: error
    }));
  };

  const handlePhoneChange = (index, field, value) => {
    let formattedValue = value;
    
    if (field === 'phoneNumber') {
      formattedValue = formatPhone(value);
    }
    
    const updatedPhones = formData.phones.map((phone, i) => {
      if (i === index) {
        return { ...phone, [field]: formattedValue };
      }
      return phone;
    });

    setFormData((prev) => ({
      ...prev,
      phones: updatedPhones,
    }));
    
    if (field === 'phoneNumber' && touched[`phone-${index}`]) {
      const error = !formattedValue.trim() ? '' : !isValidPhone(formattedValue) ? 'Telefone inválido' : '';
      setFieldErrors(prev => ({
        ...prev,
        [`phone-${index}`]: error
      }));
    }
  };
  
  const handlePhoneBlur = (index, value) => {
    setTouched(prev => ({
      ...prev,
      [`phone-${index}`]: true
    }));
    
    const error = value.trim() && !isValidPhone(value) ? 'Telefone inválido' : '';
    setFieldErrors(prev => ({
      ...prev,
      [`phone-${index}`]: error
    }));
  };

  const addPhone = () => {
    setFormData((prev) => ({
      ...prev,
      phones: [
        ...prev.phones,
        { phoneNumber: "", phoneType: "Mobile", isPrimary: false },
      ],
    }));
  };

  const removePhone = (index) => {
    if (formData.phones.length > 1) {
      setFormData((prev) => ({
        ...prev,
        phones: prev.phones.filter((_, i) => i !== index),
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    
    const errors = {};
    const requiredFields = ['firstName', 'lastName', 'email', 'docNumber', 'age', 'position', 'department', 'salary'];
    
    requiredFields.forEach(field => {
      const error = validateField(field, formData[field]);
      if (error) {
        errors[field] = error;
      }
    });
    
    formData.phones.forEach((phone, index) => {
      if (phone.phoneNumber.trim() && !isValidPhone(phone.phoneNumber)) {
        errors[`phone-${index}`] = 'Telefone inválido';
      }
    });
    
    const validPhones = formData.phones.filter(phone => phone.phoneNumber.trim() !== '');
    if (validPhones.length === 0) {
      errors.phones = 'Pelo menos um telefone é obrigatório';
    }
    
    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors);
      setTouched(requiredFields.reduce((acc, field) => ({ ...acc, [field]: true }), {}));
      showError('Por favor, corrija os erros no formulário antes de continuar.');
      return;
    }

    setLoading(true);

    try {
      const phones = formData.phones
        .filter((phone) => phone.phoneNumber.trim() !== "")
        .map((phone) => ({
          PhoneNumber: phone.phoneNumber,
          PhoneType: phone.phoneType || "Mobile",
          IsPrimary: phone.isPrimary || false,
        }));

      const employeeData = {
        ...formData,
        phones,
        age: parseInt(formData.age) || 0,
        salary: parseCurrency(formData.salary) || 0,
      };

      if (id) {
        await employeeService.update(id, employeeData);
        showSuccess('Funcionário atualizado com sucesso!');
      } else {
        await employeeService.create(employeeData);
        showSuccess('Funcionário criado com sucesso!');
      }

      navigate("/");
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.message || 'Erro desconhecido';
      
      showError(`Erro ao salvar funcionário: ${errorMessage}`);
      setError(`Erro ao salvar funcionário: ${errorMessage}`);
    } finally {
      setLoading(false);
    }
  };

  if (loading && id) {
    return (
      <div className="min-h-screen bg-gray-50 flex justify-center items-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600 text-lg">
            Carregando dados do funcionário...
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-4 px-4 sm:py-6 sm:px-6 lg:py-8 lg:px-8">
      <div className="max-w-6xl mx-auto">
        <div className="bg-white rounded-lg shadow-sm p-6 sm:p-8 lg:p-10 mb-6">
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900">
              {id ? "Editar Funcionário" : "Adicionar Funcionário"}
            </h1>
            <p className="mt-1 text-gray-600">
              {id
                ? "Atualize as informações do funcionário abaixo"
                : "Preencha as informações do novo funcionário abaixo"}
            </p>
          </div>

          {error && (
            <div
              className="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg mb-6"
              role="alert"
            >
              <div className="flex items-center">
                <svg
                  className="w-5 h-5 mr-3"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    fillRule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                    clipRule="evenodd"
                  />
                </svg>
                <span>{error}</span>
              </div>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              <FormField
                label="Nome"
                type="text"
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.firstName}
                touched={touched.firstName}
                required
              />
              
              <FormField
                label="Sobrenome"
                type="text"
                id="lastName"
                name="lastName"
                value={formData.lastName}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.lastName}
                touched={touched.lastName}
                required
              />
            </div>

            <FormField
              label="Email"
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              onBlur={handleBlur}
              error={fieldErrors.email}
              touched={touched.email}
              required
            />

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              <FormField
                label="Documento (CPF/RG)"
                type="text"
                id="docNumber"
                name="docNumber"
                value={formData.docNumber}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.docNumber}
                touched={touched.docNumber}
                placeholder="000.000.000-00"
                required
              />

              <FormField
                label="Idade"
                type="number"
                id="age"
                name="age"
                value={formData.age}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.age}
                touched={touched.age}
                min="18"
                max="100"
                required
              />
            </div>

            <FormField
              label="Cargo"
              type="text"
              id="position"
              name="position"
              value={formData.position}
              onChange={handleInputChange}
              onBlur={handleBlur}
              error={fieldErrors.position}
              touched={touched.position}
              required
            />

            <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
              <FormField
                label="Departamento"
                type="text"
                id="department"
                name="department"
                value={formData.department}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.department}
                touched={touched.department}
                required
              />

              <FormField
                label="Salário"
                type="text"
                id="salary"
                name="salary"
                value={formData.salary}
                onChange={handleInputChange}
                onBlur={handleBlur}
                error={fieldErrors.salary}
                touched={touched.salary}
                placeholder="R$ 0,00"
                required
              />
            </div>

            <div>
              <label
                htmlFor="permissionLevel"
                className="block text-sm font-medium text-gray-700"
              >
                Nível de Permissão
              </label>
              <select
                id="permissionLevel"
                name="permissionLevel"
                value={formData.permissionLevel}
                onChange={handleInputChange}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value="employee">Funcionário</option>
                <option value="manager">Gerente</option>
                <option value="admin">Administrador</option>
              </select>
            </div>

            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-medium text-gray-900">Telefones</h3>
                {fieldErrors.phones && (
                  <span className="text-sm text-red-600">{fieldErrors.phones}</span>
                )}
              </div>
              
              {formData.phones.map((phone, index) => (
                <div key={index} className="p-4 border border-gray-200 rounded-lg bg-gray-50">
                  <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
                    <div className="sm:col-span-2">
                      <FormField
                        label="Número do Telefone"
                        type="tel"
                        id={`phone-${index}`}
                        value={phone.phoneNumber}
                        onChange={(e) => handlePhoneChange(index, "phoneNumber", e.target.value)}
                        onBlur={(e) => handlePhoneBlur(index, e.target.value)}
                        error={fieldErrors[`phone-${index}`]}
                        touched={touched[`phone-${index}`]}
                        placeholder="(11) 99999-9999"
                      />
                    </div>

                    <div>
                      <label
                        htmlFor={`phoneType-${index}`}
                        className="block text-sm font-medium text-gray-700 mb-1"
                      >
                        Tipo
                      </label>
                      <select
                        id={`phoneType-${index}`}
                        value={phone.phoneType}
                        onChange={(e) =>
                          handlePhoneChange(index, "phoneType", e.target.value)
                        }
                        className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                      >
                        <option value="Mobile">Celular</option>
                        <option value="Home">Residencial</option>
                        <option value="Work">Comercial</option>
                      </select>
                    </div>
                  </div>
                  
                  <div className="flex items-center justify-between mt-4">
                    <div className="flex items-center">
                      <input
                        type="checkbox"
                        id={`isPrimary-${index}`}
                        checked={phone.isPrimary}
                        onChange={(e) =>
                          handlePhoneChange(index, "isPrimary", e.target.checked)
                        }
                        className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                      />
                      <label
                        htmlFor={`isPrimary-${index}`}
                        className="ml-2 block text-sm text-gray-900 font-medium"
                      >
                        Telefone Principal
                      </label>
                    </div>

                    {formData.phones.length > 1 && (
                      <button
                        type="button"
                        onClick={() => removePhone(index)}
                        className="inline-flex items-center px-3 py-1.5 border border-red-300 text-sm font-medium rounded-md text-red-700 bg-red-50 hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 transition-colors"
                      >
                        🗑️ Remover
                      </button>
                    )}
                  </div>
                </div>
              ))}

              <button
                type="button"
                onClick={addPhone}
                className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
              >
                📞 Adicionar Telefone
              </button>
            </div>

            <div className="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-3 pt-6 border-t border-gray-200">
              <button
                type="button"
                onClick={() => navigate("/")}
                disabled={loading}
                className="inline-flex items-center justify-center py-3 px-6 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                ❌ Cancelar
              </button>
              <button
                type="submit"
                disabled={loading}
                className="inline-flex items-center justify-center py-3 px-6 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {loading ? (
                  <>
                    <svg
                      className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                      />
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      />
                    </svg>
                    Salvando...
                  </>
                ) : (
                  <>
                    💾 {id ? 'Atualizar' : 'Criar'} Funcionário
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default EmployeeForm;