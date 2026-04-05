# Technical Challenge - Gestão Financeira

Backend + Frontend para o desafio técnico Full Stack. Gerencia Usuários, Pessoas, Categorias e Transações financeiras com autenticação JWT, validações de domínio e organização em camadas bem definidas.

 Tecnologias

Backend
- .NET 8 (C# 12)
- Entity Framework Core com SQLite
- MediatR (CQRS — Commands e Queries separados)
- FluentValidation
- BCrypt.Net (hash de senha)
- JWT Bearer Authentication (15 min de expiração)
- xUnit + Moq + FluentAssertions

Frontend
- React 19 + Vite + TypeScript
- React Router DOM (rotas privadas)
- React Hook Form + Zod
- Axios com interceptors de token e idioma
- CSS Modules + Vanilla CSS com Light/Dark mode

## Arquitetura e Decisões Técnicas

O projeto utiliza uma **Arquitetura em Camadas (Layered Architecture)** inspirada nos princípios de Clean Architecture e DDD, utilizando o padrão **CQRS** para separação de preocupações.

### Organização do Backend
1. **Domain**: Contém a lógica central, entidades de negócio e interfaces. Não possui dependências externas ou de infraestrutura.
2. **Application**: Implementa os casos de uso seguindo o padrão CQRS via **MediatR**. Separa rigorosamente Comandos (escrita) de Queries (leitura).
3. **Infrastructure**: Contém os detalhes de implementação como Repositórios (EF Core), as configurações do SQLite e serviços externos (JWT).
4. **API**: Camada de entrada (Controllers) que expõe os recursos via HTTP. Utiliza Middlewares para tratamento global de erros, autenticação e internacionalização.

### Por que essa abordagem?
- **Desacoplamento**: O domínio é isolado. Mudanças de frameworks ou persistência não afetam a regra de negócio.
- **Manutenibilidade**: A separação entre leitura e escrita permite que cada fluxo evolua independentemente.
- **Testes**: Facilita a escrita de testes unitários devido ao uso extensivo de interfaces e injeção de dependência.

### Detalhes de Performance
- **AsNoTracking**: Consultas de leitura utilizam `.AsNoTracking()` para reduzir a sobrecarga do contexto do EF Core e melhorar o tempo de resposta.
- **SQLite**: Escolhido para simplificar a portabilidade e facilitar o setup imediato do desafio técnico.

Estrutura do Projeto

```text
.
├── frontend/                # Aplicação React + Vite
│   ├── src/
│   │   ├── components/      # Componentes reutilizáveis
│   │   ├── contexts/        # Auth e Theme contexts
│   │   ├── pages/           # Telas (Login, Register, Dashboard)
│   │   └── services/        # Configuração do Axios (API)
│   └── index.html
├── src/                     # Backend .NET 8
│   ├── TechnicalChallenge.API/            # Entry point e Controllers
│   ├── TechnicalChallenge.Application/    # Casos de uso e DTOs
│   ├── TechnicalChallenge.Domain/         # Entidades e Regras de Negócio
│   ├── TechnicalChallenge.Infrastructure/ # EF Core e Repositórios
│   └── TechnicalChallenge.Shared/         # Exceções e Resultados comuns
├── tests/                   # Testes unitários e de integração
└── README.md
```

Como rodar

-Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

-Backend

```bash
cd src/TechnicalChallenge.API
dotnet run
```

Na primeira execução o banco SQLite ('technical-challenge.db') é criado automaticamente na raiz da API.

Swagger disponível em: 'https://localhost:63272/swagger/index.html'

Health Check disponível em: 'https://localhost:63272/api/health'

-Frontend

```bash
cd frontend
npm install
npm run dev
```

Acesse em: 'http://localhost:5173'

> Certifique-se de que a API esteja rodando antes de usar o frontend.

-Testes

```bash
dotnet test
```

Autenticação

Dois endpoints públicos:

```
POST /api/auth/register   → cria conta, retorna token JWT (expira em 15min)
POST /api/auth/login      → autentica, retorna token JWT (expira em 15min)
```

Demais rotas exigem o header:
```
Authorization: Bearer <token>
```

-Internacionalização

As mensagens de erro da API respondem no idioma do header 'Accept-Language'.
O frontend envia esse header automaticamente com base no idioma selecionado pelo usuário.

Disponível: 'pt-BR', 'en-US' e 'es-ES'
