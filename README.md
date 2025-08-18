# 🏢 Sistema de Gestão de Funcionários - Recruitment Process

Um sistema completo de gestão de funcionários desenvolvido com arquitetura de microserviços, utilizando .NET 8, React e Docker.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Como Executar](#como-executar)
- [Endpoints da API](#endpoints-da-api)
- [Banco de Dados](#banco-de-dados)
- [Autenticação](#autenticação)
- [Testes](#testes)
- [Documentação da API](#documentação-da-api)

## 🎯 Sobre o Projeto

Este projeto é um sistema completo de gestão de funcionários que implementa as melhores práticas de desenvolvimento de software, incluindo:

- **Arquitetura de Microserviços** com API Gateway
- **Autenticação JWT** com controle de permissões hierárquicas
- **Frontend React** moderno e responsivo
- **Containerização completa** com Docker
- **Testes unitários** e documentação da API
- **Logs estruturados** com Serilog
- **Validações de negócio** robustas

### 🏗️ Arquitetura

```mermaid
graph TB
    subgraph "Frontend"
        React[React App<br/>Port: 3000]
    end
    
    subgraph "API Gateway"
        Gateway[API Gateway<br/>Port: 5001]
    end
    
    subgraph "Microservices"
        Identity[Identity Service<br/>Port: 5047]
        Employee[Employee Service<br/>Port: 5022]
    end
    
    subgraph "Databases"
        IdentityDB[(Identity DB<br/>PostgreSQL<br/>Port: 5433)]
        EmployeeDB[(Employee DB<br/>PostgreSQL<br/>Port: 5434)]
        Redis[(Redis Cache<br/>Port: 6379)]
    end
    
    React --> Gateway
    Gateway --> Identity
    Gateway --> Employee
    Identity --> IdentityDB
    Employee --> EmployeeDB
    Employee --> Redis
    Identity --> Redis
```

### 🔧 Estrutura do Projeto

```
RecruitmentProcess/
├── Backend/
│   ├── ApiGateway/
│   │   └── ApiGateway.API/          # Gateway de roteamento
│   ├── EmployeeService/
│   │   ├── EmployeeService.API/     # API de funcionários
│   │   └── EmployeeService.Tests/   # Testes unitários
│   └── IdentityService/
│       ├── IdentityService.API/     # API de autenticação
│       └── IdentityService.Tests/   # Testes unitários
├── Frontend/
│   └── recruitment-app/             # Aplicação React
├── Docker/
│   └── docker-compose.yml           # Orquestração dos containers
└── README.md
```

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - APIs REST
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados principal
- **Redis** - Cache e sessões
- **JWT** - Autenticação e autorização
- **Serilog** - Logs estruturados
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Testes unitários
- **Ocelot** - API Gateway

### Frontend
- **React 18** - Framework frontend
- **React Router DOM** - Roteamento
- **Tailwind CSS** - Estilização
- **Axios** - Cliente HTTP
- **Context API** - Gerenciamento de estado

### DevOps
- **Docker** - Containerização
- **Docker Compose** - Orquestração
- **Nginx** - Servidor web para React

## ✨ Funcionalidades

### 🔐 Autenticação e Autorização
- Login e registro de usuários
- Autenticação JWT com refresh tokens
- Controle de permissões hierárquicas:
  - **Employee**: Pode visualizar funcionários
  - **Leader**: Pode criar/editar funcionários (exceto Directors)
  - **Director**: Acesso completo ao sistema

### 👥 Gestão de Funcionários
- **CRUD completo** de funcionários
- **Validações de negócio**:
  - Idade mínima de 16 anos
  - Validação de hierarquia de permissões
  - Validação de dados obrigatórios
- **Múltiplos telefones** por funcionário
- **Histórico de alterações**
- **Busca e filtros** avançados

### 🎨 Interface do Usuário
- Design moderno e responsivo
- Navegação intuitiva
- Feedback visual para ações
- Formulários com validação em tempo real
- Tabelas com paginação e ordenação

## 🏃‍♂️ Como Executar

### 📋 Pré-requisitos

- **Docker Desktop** instalado e rodando
- **Git** para clonar o repositório
- **Portas disponíveis**: 3000, 5001, 5022, 5047, 5433, 5434, 6379

### 🐳 Executar com Docker (Recomendado)

1. **Clone o repositório:**
```bash
git clone <repository-url>
cd RecruitmentProcess
```

2. **Execute todos os serviços:**
```bash
cd Docker
docker-compose up -d
```

3. **Aguarde a inicialização** (aproximadamente 2-3 minutos)

4. **Acesse os serviços:**
   - **Frontend:** http://localhost:3000
   - **API Gateway:** http://localhost:5001
   - **Swagger (Identity):** http://localhost:5047/swagger
   - **Swagger (Employee):** http://localhost:5022/swagger

### 🔧 Executar Manualmente (Desenvolvimento)

#### Backend

1. **Instale o .NET 8 SDK**

2. **Configure os bancos de dados:**
```bash
# PostgreSQL deve estar rodando nas portas 5433 e 5434
# Redis deve estar rodando na porta 6379
```

3. **Execute os serviços:**
```bash
# Identity Service
cd Backend/IdentityService/IdentityService.API
dotnet run

# Employee Service (novo terminal)
cd Backend/EmployeeService/EmployeeService.API
dotnet run

# API Gateway (novo terminal)
cd Backend/ApiGateway/ApiGateway.API
dotnet run
```

#### Frontend

```bash
cd Frontend/recruitment-app
npm install
npm start
```

### 👤 Usuário Padrão

O sistema cria automaticamente um usuário administrador:

**Credenciais:**
- **Email:** admin@admin.com
- **Senha:** Admin123!
- **Permissão:** Director (acesso completo)

## 🌐 Endpoints da API

### 🔐 Autenticação (Identity Service)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/api/auth/register` | Registrar novo usuário | ❌ |
| POST | `/api/auth/login` | Login do usuário | ❌ |

**Exemplo de Login:**
```json
POST /api/auth/login
{
  "email": "admin@admin.com",
  "password": "Admin123!"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-01-01T12:00:00Z",
  "user": {
    "id": "123",
    "firstName": "Admin",
    "lastName": "User",
    "email": "admin@admin.com"
  }
}
```

### 👥 Funcionários (Employee Service)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/employees` | Listar funcionários | ✅ |
| GET | `/api/employees/{id}` | Obter funcionário por ID | ✅ |
| POST | `/api/employees` | Criar funcionário | ✅ |
| PUT | `/api/employees/{id}` | Atualizar funcionário | ✅ |
| DELETE | `/api/employees/{id}` | Deletar funcionário | ✅ |

**Exemplo de Criação:**
```json
POST /api/employees
Authorization: Bearer {token}
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao.silva@empresa.com",
  "docNumber": "12345678901",
  "age": 30,
  "position": "Desenvolvedor",
  "department": "TI",
  "salary": 8000.00,
  "hireDate": "2024-01-15T00:00:00Z",
  "permissionLevel": 1,
  "phones": [
    {
      "phoneNumber": "(11) 99999-9999",
      "phoneType": "Mobile",
      "isPrimary": true
    }
  ]
}
```

### 📊 Status

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/status` | Status do serviço | ❌ |

## 🗄️ Banco de Dados

### PostgreSQL - Identity Service (Porta 5433)
```sql
-- Tabelas principais
- AspNetUsers          -- Usuários do sistema
- AspNetRoles          -- Roles/Permissões
- AspNetUserRoles      -- Relacionamento usuário-role
```

### PostgreSQL - Employee Service (Porta 5434)
```sql
-- Tabelas principais
- Employees            -- Dados dos funcionários
- EmployeePhones       -- Telefones dos funcionários
- Users                -- Cache de usuários para validação
```

### Redis (Porta 6379)
- **Cache de sessões**
- **Cache de dados temporários**
- **Rate limiting**

## 🔒 Autenticação

### JWT Token
- **Algoritmo:** HS256
- **Expiração:** 60 minutos
- **Issuer:** IdentityService
- **Audience:** RecruitmentApp

### Hierarquia de Permissões
```
Director (3)    ← Pode criar qualquer funcionário
    ↑
Leader (2)      ← Pode criar Employee e Leader
    ↑
Employee (1)    ← Apenas visualização
```

### Headers de Autenticação
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🧪 Testes

### Executar Testes Unitários
```bash
# Todos os testes
dotnet test

# Testes específicos
cd Backend/EmployeeService/EmployeeService.Tests
dotnet test

cd Backend/IdentityService/IdentityService.Tests
dotnet test
```

### Cobertura de Testes
- **Services:** 85%+
- **Controllers:** 80%+
- **Models/DTOs:** 90%+

## 📚 Documentação da API

### Swagger UI
- **Identity Service:** http://localhost:5047/swagger
- **Employee Service:** http://localhost:5022/swagger
- **API Gateway:** http://localhost:5001/swagger

### Recursos do Swagger
- **Documentação interativa** de todos os endpoints
- **Teste direto** das APIs
- **Esquemas de dados** detalhados
- **Autenticação JWT** integrada

## 🔧 Configurações

### Variáveis de Ambiente
```bash
# Database
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Database=...

# JWT
JwtSettings__Secret=super-secret-key...
JwtSettings__Issuer=IdentityService
JwtSettings__Audience=RecruitmentApp
JwtSettings__ExpiryMinutes=60
```

### CORS
```csharp
// Configurado para aceitar requisições do React
WithOrigins("http://localhost:3000")
```

## 📝 Logs

### Estrutura de Logs
- **Console:** Desenvolvimento
- **Arquivo:** `logs/service-name-YYYY-MM-DD.txt`
- **Formato:** JSON estruturado

### Níveis de Log
- **Information:** Operações normais
- **Warning:** Situações de atenção
- **Error:** Erros de aplicação
- **Critical:** Falhas críticas

## 🚀 Deploy e Produção

### Docker Production
```bash
# Build para produção
docker-compose -f docker-compose.prod.yml up -d
```

### Considerações de Produção
- **HTTPS** obrigatório
- **Secrets** em variáveis de ambiente
- **Load balancer** para múltiplas instâncias
- **Monitoramento** com Prometheus/Grafana
- **Backup** automático dos bancos

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

**Desenvolvido com ❤️ para o processo de recrutamento**