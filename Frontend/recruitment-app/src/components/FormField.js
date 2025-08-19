import React from 'react';

const FormField = ({
  label,
  type = 'text',
  id,
  name,
  value,
  onChange,
  onBlur,
  error,
  touched,
  required = false,
  placeholder,
  className = '',
  icon,
  ...props
}) => {
  const hasError = touched && error;
  const isValid = touched && !error && value;

  return (
    <div className={`mb-4 ${className}`}>
      <label htmlFor={id} className="block text-sm font-medium text-gray-700 mb-1">
        {label}
        {required && <span className="text-red-500 ml-1">*</span>}
      </label>
      
      <div className="relative">
        {icon && (
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            {icon}
          </div>
        )}
        
        <input
          type={type}
          id={id}
          name={name}
          value={value}
          onChange={onChange}
          onBlur={onBlur}
          placeholder={placeholder}
          required={required}
          className={`
            block w-full rounded-md shadow-sm transition-all duration-200 ease-in-out
            ${icon ? 'pl-10' : 'pl-3'} pr-10 py-2.5
            ${hasError 
              ? 'border-red-300 focus:border-red-500 focus:ring-red-500' 
              : isValid
                ? 'border-green-300 focus:border-green-500 focus:ring-green-500'
                : 'border-gray-300 focus:border-blue-500 focus:ring-blue-500 hover:border-gray-400'
            }
            focus:ring-2 focus:ring-opacity-50
          `}
          {...props}
        />
        
        {/* Indicadores de validação */}
         {isValid && (
           <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
             <svg className="h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
               <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
             </svg>
           </div>
         )}
         
         {hasError && (
           <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
             <svg className="h-5 w-5 text-red-500" fill="currentColor" viewBox="0 0 20 20">
               <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
             </svg>
           </div>
         )}
      </div>
      
      {hasError && (
         <p className="mt-1 text-sm text-red-600">{error}</p>
       )}
    </div>
  );
};

export default FormField;