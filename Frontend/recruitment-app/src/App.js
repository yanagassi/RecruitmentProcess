import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import EmployeeList from './pages/EmployeeList';
import EmployeeForm from './pages/EmployeeForm';
import Login from './pages/Login';
import Register from './pages/Register';
import ProtectedRoute from './components/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import { ToastProvider } from './contexts/ToastContext';

function App() {
  return (
    <Router>
      <AuthProvider>
        <ToastProvider>
          <div className="min-h-screen bg-gray-100">
            <Navbar />
            <div className="container mx-auto px-4 py-8">
              <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/" element={
                  <ProtectedRoute>
                    <EmployeeList />
                  </ProtectedRoute>
                } />
                <Route path="/employees/add" element={
                  <ProtectedRoute>
                    <EmployeeForm />
                  </ProtectedRoute>
                } />
                <Route path="/employees/edit/:id" element={
                  <ProtectedRoute>
                    <EmployeeForm />
                  </ProtectedRoute>
                } />
              </Routes>
            </div>
          </div>
        </ToastProvider>
      </AuthProvider>
    </Router>
  );
}

export default App;