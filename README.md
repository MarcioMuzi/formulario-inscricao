# Formulário de Inscrição (.NET 8 + Blazor Server + SQL Server + Identity + Docker)

[![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/CSharp-12-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-12)
[![Blazor Server](https://img.shields.io/badge/Blazor-Server-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://learn.microsoft.com/aspnet/core/blazor/?view=aspnetcore-8.0)
[![MudBlazor 8.12.0](https://img.shields.io/badge/MudBlazor-8.12.0-6E59A5?style=for-the-badge)](https://mudblazor.com/)
[![EF Core 9.0.8](https://img.shields.io/badge/EF%20Core-9.0.8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![ASP.NET Identity](https://img.shields.io/badge/ASP.NET%20Identity-8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
[![SQL Server 2022](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server/sql-server-2022)
[![Docker Compose](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://docs.docker.com/compose/)

Projeto de exemplo utilizando **.NET 8**, **Blazor Server (Razor Components)**, **MudBlazor**, **Entity Framework Core 9** (SQL Server) e **ASP.NET Core Identity (cookie)**.  
Serve como base para um CRUD simples de inscrições, com autenticação, **execução via Docker Compose** e boas práticas mínimas para **portfólio**.

> **Objetivo:** Blazor Server **sem** expor APIs; autenticação por **cookie** (UI do Identity). JWT/OAuth2 pode ser adicionado como etapa posterior.

---

## 🚀 Stack

- **.NET 8 / ASP.NET Core**
- **Blazor Server** (Razor Components)
- **MudBlazor** (UI) – v8.12.0
- **Entity Framework Core** – v9.0.8 (SQL Server)
- **ASP.NET Core Identity** (cookie + UI padrão)
- **Docker Compose** (Web + SQL Server)
- **C# 12**

---

## ✨ Funcionalidades

- **/enroll**: Formulário para criar inscrições (validação com DataAnnotations, UX com loading/disable e Snackbar).
- **/enrollments**: Listagem (MudTable) das últimas inscrições, **protegida** por autenticação.
- **Autenticação**: Registro/Login via **UI padrão do Identity**.
- **Persistência**: SQL Server via EF Core (migrations versionadas).
- **Estabilidade MudBlazor**:
  - CSS em `<HeadContent>`
  - **`_content/MudBlazor/MudBlazor.min.js`** adicionado (JSInterop do MudBlazor).
- **Docker**:
  - `web`: aplicação ASP.NET
  - `sql`: SQL Server 2022
  - **Chaves de Data Protection persistidas** (evita erros de antiforgery após restart).

---

## 🗂️ Estrutura do projeto (resumo)

```
src/WebApp
  ├── Components
  │   ├── App.razor
  │   ├── Routes.razor
  │   ├── Layout/
  │   │   ├── MainLayout.razor
  │   │   └── NavMenu.razor
  │   └── Pages/
  │       ├── Enroll.razor          # formulário (criação)
  │       └── Enrollments.razor     # listagem (autorizada)
  ├── Areas/
  │   └── Identity/
  │       └── Pages/
  │           └── _Layout.cshtml    # layout local p/ UI do Identity
  ├── Pages/
  │   └── Shared/_LoginPartial.cshtml  # parcial (fallback)
  ├── Views/
  │   └── Shared/_LoginPartial.cshtml  # parcial (fallback)
  ├── Data/
  │   ├── AppDbContext.cs           # herda de IdentityDbContext
  │   └── Enrollment.cs             # entidade domínio
  ├── Migrations/                   # InitialCreate, AddEnrollmentFields, AddIdentity etc.
  ├── wwwroot/
  ├── Program.cs
  ├── WebApp.csproj
  ├── docker-compose.yml
  └── Dockerfile
```

---

## 🔐 Autenticação & Autorização

- **Identity (cookie)** registrado com `AddDefaultIdentity<IdentityUser>()` e UI (Razor Class Library) mapeada via `app.MapRazorPages()`.
- **Blazor**: `Components/Routes.razor` usa `CascadingAuthenticationState` + `AuthorizeRouteView`.  
  Páginas podem ser protegidas com:
  ```razor
  @attribute [Microsoft.AspNetCore.Authorization.Authorize]
  ```
- **URLs úteis**:
  - Registro: `http://localhost:5090/Identity/Account/Register`
  - Login: `http://localhost:5090/Identity/Account/Login`

> Para evitar dependência do parcial `_LoginPartial` da RCL, o projeto sobrescreve o layout da área Identity em `Areas/Identity/Pages/_Layout.cshtml`.

---

## 🧱 Banco de Dados & Migrations

- **Banco**: `CourseEnroll`
- **Entidade**: `Enrollment { Id, FullName, Email, Course?, CreatedAt }`
- **Migrations** versionadas em `Migrations/`.

### Aplicar migrations **apontando para o SQL do Docker** (porta 14333)

> **Git Bash / Linux / Mac** (variáveis inline):

```bash
cd src/WebApp
ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'   dotnet ef database update
```

> **Observação:** o container `web` já recebe a connection string pelo `docker-compose.yml` usando host `sql` (rede interna do Compose). O comando acima é apenas para aplicar/atualizar o schema **do host** na instância do container `sql`.

---

## 🐳 Docker (Compose)

### `docker-compose.yml` (resumo)

- **sql**: `mcr.microsoft.com/mssql/server:2022-latest`
  - Porta mapeada: `14333:1433`
  - Healthcheck com `sqlcmd`
  - Volume: `mssql_data:/var/opt/mssql`
- **web**: build do `Dockerfile`, porta `5090:8080`
  - `ASPNETCORE_ENVIRONMENT=Development`
  - `ConnectionStrings__DefaultConnection=Server=sql,1433;...`
  - **Volume** `web_keys:/root/.aspnet/DataProtection-Keys` (💡 evita warnings/exceções de antiforgery após restart)

### Subir tudo

```bash
cd src/WebApp
docker compose build
SA_PASSWORD='Pass@w0rd!' docker compose up -d
docker compose ps
```

> **Primeiro uso:** aplique as migrations do host (comando da seção anterior).

### Logs

```bash
docker compose logs -f web
docker compose logs -f sql
```

### Reiniciar somente o web (após mudanças no código)

```bash
docker compose build web && docker compose up -d web
```

### Derrubar (⚠️ remove volume do banco)

```bash
docker compose down -v
```

---

## ▶️ Executando **sem** Docker (opcional)

```bash
cd src/WebApp
dotnet build
# (opcional) exporte a connection string real via user-secrets ou appsettings.Development.json
dotnet run
# http://localhost:5090
```

> Em **desenvolvimento** o projeto usa HTTP; redirecionamento para HTTPS está habilitado apenas fora de `Development`.

---

## 🧩 Dicas MudBlazor

- Inclua o **CSS** no `<HeadContent>` e o **JS** do MudBlazor **após** `blazor.web.js`:
  ```razor
  <HeadContent>
    <link rel="stylesheet" href="_content/MudBlazor/MudBlazor.min.css" />
  </HeadContent>

  ...
  <script src="_framework/blazor.web.js"></script>
  <script src="_content/MudBlazor/MudBlazor.min.js"></script>
  ```
- Providers recomendados no layout:
  ```razor
  <MudThemeProvider />
  <MudPopoverProvider />
  <MudDialogProvider />
  <MudSnackbarProvider />
  ```
  **Atenção:** **apenas um** `MudPopoverProvider` por layout para evitar erros de overlay/popover.

---

## 🔄 Fluxo de Git (sugerido)

- **main**: apenas merge da **develop** após homologação.
- **develop**: integração/testes.
- **feature branches**: sempre criadas a partir da **develop**.  
  Ex.: `feat/identity-cookie`, `feat/enroll-validation`, `chore/devops-compose`.

Comandos úteis (primeiro push com upstream):
```bash
git switch develop
git pull --ff-only origin develop
git switch -c feat/minha-feature

# trabalho...
git add -A
git commit -m "feat: descrição curta"
git push -u origin HEAD
```

---

## ✅ Checklist de Validação Rápida

1. `docker compose up -d` (com `SA_PASSWORD` definido)  
2. `dotnet ef database update` apontando para `localhost,14333`  
3. Acessar `http://localhost:5090/Identity/Account/Register` → criar usuário  
4. Login e abrir `http://localhost:5090/enrollments` (deve exigir login)  
5. Criar inscrição em `http://localhost:5090/enroll` e verificar na listagem

---

## 📌 Roadmap (opcional)

- Novos campos em `Enrollment` (VM, validação, migration).
- **CI/CD** (Azure DevOps): build/test e publish de imagem Docker.
- **User Secrets** local (quando rodar sem Docker).
- JWT/OAuth2 (IdentityServer/OpenIddict) **apenas** se necessário para clientes externos.

---

## 📝 Licença

MIT. Use à vontade em estudos, entrevistas e POCs.
