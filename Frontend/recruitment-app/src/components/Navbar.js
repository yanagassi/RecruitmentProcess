import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
  const { isAuthenticated, logout, user } = useAuth();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen(!isMobileMenuOpen);
  };

  const closeMobileMenu = () => {
    setIsMobileMenuOpen(false);
  };

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex justify-between items-center py-4">
          <div className="text-xl font-bold">
            <Link to="/" className="hover:text-blue-200" onClick={closeMobileMenu}>
              <span className="hidden sm:inline">Sistema de Gestão de Funcionários</span>
              <span className="sm:hidden">SGF</span>
            </Link>
          </div>

          <div className="hidden md:flex space-x-4 items-center">
            {isAuthenticated() ? (
              <>
                <Link to="/" className="hover:text-blue-200 transition duration-200">
                  Funcionários
                </Link>
                <Link to="/employees/add" className="hover:text-blue-200 transition duration-200">
                  Adicionar Funcionário
                </Link>
                <span className="text-blue-200 text-sm">
                  Olá, {user?.firstName || 'Usuário'}
                </span>
                <button
                  onClick={logout}
                  className="bg-red-500 hover:bg-red-600 px-4 py-2 rounded-md transition duration-300 text-sm font-medium"
                >
                  Sair
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className="hover:text-blue-200 transition duration-200"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="bg-white text-blue-600 hover:bg-blue-100 px-4 py-2 rounded-md transition duration-300 text-sm font-medium"
                >
                  Registrar
                </Link>
              </>
            )}
          </div>

          <div className="md:hidden">
            <button
              onClick={toggleMobileMenu}
              className="text-white hover:text-blue-200 focus:outline-none focus:text-blue-200 transition duration-200"
              aria-label="Toggle mobile menu"
            >
              <svg
                className="h-6 w-6"
                fill="none"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                {isMobileMenuOpen ? (
                  <path d="M6 18L18 6M6 6l12 12" />
                ) : (
                  <path d="M4 6h16M4 12h16M4 18h16" />
                )}
              </svg>
            </button>
          </div>
        </div>

        <div className={`md:hidden transition-all duration-300 ease-in-out ${
          isMobileMenuOpen 
            ? 'max-h-96 opacity-100 pb-4' 
            : 'max-h-0 opacity-0 overflow-hidden'
        }`}>
          <div className="border-t border-blue-500 pt-4">
            {isAuthenticated() ? (
              <div className="space-y-3">
                <div className="px-4 py-2 text-blue-200 text-sm border-b border-blue-500 pb-3">
                  Olá, {user?.firstName || 'Usuário'}
                </div>
                <Link 
                  to="/" 
                  className="block px-4 py-2 hover:bg-blue-700 rounded-md transition duration-200"
                  onClick={closeMobileMenu}
                >
                  Funcionários
                </Link>
                <Link 
                  to="/employees/add" 
                  className="block px-4 py-2 hover:bg-blue-700 rounded-md transition duration-200"
                  onClick={closeMobileMenu}
                >
                  Adicionar Funcionário
                </Link>
                <button
                  onClick={() => {
                    logout();
                    closeMobileMenu();
                  }}
                  className="w-full text-left px-4 py-2 bg-red-500 hover:bg-red-600 rounded-md transition duration-300 font-medium"
                >
                  Sair
                </button>
              </div>
            ) : (
              <div className="space-y-3">
                <Link
                  to="/login"
                  className="block px-4 py-2 hover:bg-blue-700 rounded-md transition duration-200"
                  onClick={closeMobileMenu}
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="block px-4 py-2 bg-white text-blue-600 hover:bg-blue-100 rounded-md transition duration-300 font-medium"
                  onClick={closeMobileMenu}
                >
                  Registrar
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;