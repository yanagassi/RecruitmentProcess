# Sistema de Gestão de Recrutamento

Este projeto é um sistema de gestão de recrutamento que consiste em uma API REST em .NET e um frontend em React com Tailwind CSS.

## Estrutura do Projeto

```
RecruitmentProcess/
├── Backend/
│   ├── ApiGateway/
│   │   └── ApiGateway.API/
│   ├── EmployeeService/
│   │   └── EmployeeService.API/
│   └── IdentityService/
│       └── IdentityService.API/
├── Docker/
└── Frontend/
    └── recruitment-app/
        ├── public/
        └── src/
            ├── components/
            ├── context/
            ├── pages/
            └── services/
```

## Tecnologias Utilizadas

### Backend
- .NET 6 (com planos para migrar para .NET 8)
- ASP.NET Core Web API
- Entity Framework Core
- JWT para autenticação
- Arquitetura de Microserviços

### Frontend
- React
- React Router DOM
- Tailwind CSS
- Axios para comunicação com a API

## Funcionalidades

### Backend
- Autenticação e autorização com JWT
- CRUD de funcionários
- Validações de dados
- API Gateway para gerenciar requisições entre microserviços

### Frontend
- Login e registro de usuários
- Listagem de funcionários
- Adição, edição e remoção de funcionários
- Rotas protegidas
- Interface responsiva com Tailwind CSS

## Usuário Padrão

O sistema possui um usuário administrador padrão criado automaticamente via migração:

**Credenciais de Acesso:**
- **Email:** admin@admin.com
- **Senha:** Admin123!
- **Nome:** Admin User

Este usuário é criado automaticamente quando as migrações são aplicadas no banco de dados.

## Como Executar

### Executar com Docker (Recomendado)

1. Certifique-se de que o Docker Desktop está rodando

2. Navegue até a pasta Docker:
```bash
cd RecruitmentProcess/Docker
```

3. Execute todos os serviços:
```bash
docker-compose up -d
```

4. Acesse os serviços:
   - **API Gateway:** http://localhost:5001
   - **Frontend React:** http://localhost:3000
   - **Identity Service:** http://localhost:5047
   - **Employee Service:** http://localhost:5022

### Executar Manualmente

#### Frontend

1. Navegue até a pasta do frontend:
```bash
cd RecruitmentProcess/Frontend/recruitment-app
```

2. Instale as dependências:
```bash
npm install
```

3. Execute o projeto:
```bash
npm start
```

#### Backend

1. Navegue até a pasta do serviço desejado:
```bash
cd RecruitmentProcess/Backend/IdentityService/IdentityService.API
```

2. Execute o projeto:
```bash
dotnet run
```

## Endpoints da API

### Autenticação (via API Gateway)
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/register` - Registro de novo usuário

### Funcionários (via API Gateway)
- `GET /api/employees` - Listar funcionários
- `GET /api/employees/{id}` - Obter funcionário por ID
- `POST /api/employees` - Criar funcionário
- `PUT /api/employees/{id}` - Atualizar funcionário
- `DELETE /api/employees/{id}` - Deletar funcionário

## Banco de Dados

O sistema utiliza PostgreSQL com bancos separados para cada microserviço:
- **identity_db** (porta 5433) - Dados de usuários e autenticação
- **employee_db** (porta 5434) - Dados de funcionários
- **Redis** (porta 6379) - Cache e sessões

## Próximos Passos

- ✅ Configurar Docker e Docker Compose
- ✅ Criar usuário padrão via migração
- Implementar testes unitários
- Documentar a API com Swagger
- Configurar sistema de logs
- Migrar para .NET 8