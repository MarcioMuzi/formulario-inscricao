# GUIA DE CRIAÇÃO DO AMBIENTE
_Formulário de Inscrição (.NET 8 + Blazor Server + SQL Server + Identity + Docker)_

> Este guia descreve **duas formas** de executar o projeto: **via Docker Compose (recomendado)** e **localmente sem Docker**.  
> Padrão arquitetural: **Blazor Server sem APIs**, autenticação por **cookie** com **ASP.NET Core Identity** e UI com **MudBlazor**.

---

## 1) Pré-requisitos

- **Git** + **Git Bash** (Windows) ou Terminal (macOS/Linux)
- **.NET 8 SDK** — verifique: `dotnet --version` (deve começar com `8.`)
- **Docker Desktop** (para a execução recomendada)
- (Opcional) **VS Code** + extensões *C# Dev Kit* e *C#*  
- (Opcional) **CLI do EF Core** para aplicar migrations a partir do host:
  ```bash
  dotnet tool update -g dotnet-ef || dotnet tool install -g dotnet-ef
  ```

---

## 2) Clonar o repositório

```bash
# escolha uma pasta de trabalho
cd ~/dev

# clonar
git clone https://github.com/MarcioMuzi/formulario-inscricao.git

# entrar no projeto
cd formulario-inscricao/src/WebApp
```

---

## 3) Executar com **Docker** (recomendado)

O repositório inclui `docker-compose.yml` e `Dockerfile` em `src/WebApp/`.

### 3.1 Subir os containers
```bash
docker compose build
SA_PASSWORD='Pass@w0rd!' docker compose up -d
docker compose ps
```

Serviços:
- **web** — ASP.NET (porta **5090** → 8080 no container)  
  Acesse: `http://localhost:5090`
- **sql** — SQL Server 2022 (porta **14333** → 1433 no container)

> O serviço `web` já usa a connection string apontando para o host **sql** (rede interna do Compose).

### 3.2 Aplicar as migrations (a partir do **host**, para o DB do container)
> Necessita `dotnet-ef` no host.

```bash
ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'   dotnet ef database update
```

### 3.3 Testar no navegador
- Registro: `http://localhost:5090/Identity/Account/Register`
- Login: `http://localhost:5090/Identity/Account/Login`
- Formulário: `http://localhost:5090/enroll`
- Listagem (protegida): `http://localhost:5090/enrollments`

> **Não há usuário padrão** — cadastre-se via `/Identity/Account/Register`.

### 3.4 Manutenção e logs
```bash
docker compose logs -f web
docker compose logs -f sql

# rebuild apenas do web após alterações no código
docker compose build web && docker compose up -d web

# derrubar TUDO e remover volumes (⚠️ apaga o banco)
docker compose down -v
```

### 3.5 Notas importantes (Docker)
- **Chaves de DataProtection persistidas**: o `web` monta um volume em `/root/.aspnet/DataProtection-Keys`, evitando avisos/erros de antiforgery após reinício do container.  
- Caso veja mensagem de cookie antiforgery inválido após reinício, **limpe os cookies** do site no navegador (ou reinicie o `web` com `--force-recreate`).

---

## 4) Executar **sem** Docker (opcional)

### 4.1 Connection string local (sem Docker)
Crie o arquivo `appsettings.Development.json` (está no `.gitignore`) **ou** use **user-secrets**:
```bash
# opção A — arquivo local
cat > appsettings.Development.json << 'JSON'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CourseEnroll;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
JSON

# opção B — user-secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CourseEnroll;Trusted_Connection=True;TrustServerCertificate=True"
```

### 4.2 Aplicar migrations e executar
```bash
dotnet ef database update
dotnet run
# http://localhost:5090
```

> Em `Development` a aplicação usa **HTTP**; redirecionamento para HTTPS está habilitado apenas fora de `Development`.

---

## 5) Estrutura do projeto (resumo)

```
src/WebApp
  ├── Components/
  │   ├── App.razor
  │   ├── Routes.razor                     # CascadingAuthenticationState + AuthorizeRouteView
  │   ├── Layout/
  │   │   ├── MainLayout.razor             # MudTheme/Popover/Dialog/Snackbar providers
  │   │   └── NavMenu.razor
  │   └── Pages/
  │       ├── Enroll.razor                 # formulário (validação + UX)
  │       └── Enrollments.razor            # listagem (protegida)
  ├── Areas/Identity/Pages/_Layout.cshtml  # layout local p/ UI do Identity
  ├── Pages/Shared/_LoginPartial.cshtml    # parcial (fallback)
  ├── Views/Shared/_LoginPartial.cshtml    # parcial (fallback)
  ├── Data/
  │   ├── AppDbContext.cs                  # herda de IdentityDbContext
  │   └── Enrollment.cs
  ├── Migrations/                          # InitialCreate, AddIdentity, etc.
  ├── wwwroot/
  ├── Program.cs
  ├── WebApp.csproj
  ├── docker-compose.yml
  └── Dockerfile
```

---

## 6) EF Core (comandos úteis)

Criar migration (ex.: novos campos no `Enrollment`):
```bash
dotnet ef migrations add AddEnrollmentFields -c AppDbContext
```

Aplicar no **banco do container** (porta 14333) a partir do **host**:
```bash
ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'   dotnet ef database update
```

Outros:
```bash
dotnet ef migrations list
dotnet ef database update NomeDaMigrationAnterior   # rollback
```

---

## 7) MudBlazor — pontos de atenção

- Em `App.razor`, inclua:
  ```razor
  <HeadContent>
    <link rel="stylesheet" href="_content/MudBlazor/MudBlazor.min.css" />
  </HeadContent>

  <!DOCTYPE html>
  <html lang="en">
  <head> ... <HeadOutlet /> </head>
  <body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="_content/MudBlazor/MudBlazor.min.js"></script> <!-- JSInterop do MudBlazor -->
  </body>
  </html>
  ```

- No layout (`MainLayout.razor`), **apenas um** conjunto de providers:
  ```razor
  <MudThemeProvider />
  <MudPopoverProvider />
  <MudDialogProvider />
  <MudSnackbarProvider />
  ```

- Diagnóstico de duplicidade/ausência (na raiz `src/WebApp`):
  ```bash
  grep -RInE --include='*.razor' -e '<SectionOutlet' -e 'mud-overlay-to-popover-provider' -e '<MudPopoverProvider' .
  ```

---

## 8) Autenticação & Autorização — resumo

- `Program.cs`:
  ```csharp
  builder.Services.AddDefaultIdentity<IdentityUser>()
      .AddRoles<IdentityRole>()
      .AddEntityFrameworkStores<AppDbContext>();
  app.UseAuthentication();
  app.UseAuthorization();
  app.MapRazorPages(); // UI do Identity
  ```

- `Components/Routes.razor`: `CascadingAuthenticationState` + `AuthorizeRouteView`  
- Para proteger uma página do Blazor:
  ```razor
  @attribute [Microsoft.AspNetCore.Authorization.Authorize]
  ```

> O layout da área Identity é sobrescrito localmente em `Areas/Identity/Pages/_Layout.cshtml` (evita dependência do `_LoginPartial` da RCL).

---

## 9) Troubleshooting

### 9.1 Antiforgery / DataProtection
- Sintoma: *“The antiforgery token could not be decrypted”* após reiniciar containers.  
- Solução: volume `web_keys` já configurado. Se persistir, recrie o web:
  ```bash
  docker compose up -d --force-recreate web
  # e limpe os cookies do site no navegador
  ```

### 9.2 JSInterop do MudBlazor
- Sintoma: *“Could not find 'mudElementRef.addOnBlurEvent'”*.  
- Verifique na aba **Network** se `/_content/MudBlazor/MudBlazor.min.js` carrega com **200**.
- Confirme que o script do MudBlazor vem **depois** de `blazor.web.js`.

### 9.3 Providers duplicados
- Sintoma: *“There is already a subscriber ... 'mud-overlay-to-popover-provider'”*.  
- Remova `<SectionOutlet ...>` e garanta **um único** `<MudPopoverProvider />`.

---

## 10) Fluxo de Git (sugerido)

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feat/minha-feature
# ... trabalho, commits ...
git push -u origin HEAD
# MR -> develop -> (após homologar) -> main
```

---

## 11) Portas & credenciais

- **Web**: `http://localhost:5090`
- **SQL**: `localhost:14333` (usuário `sa`, senha `SA_PASSWORD`, ex.: `Pass@w0rd!`)
- **Login**: cadastre via `/Identity/Account/Register`

---

## 12) Limpeza completa

```bash
docker compose down -v
# remove containers, rede e volumes (inclusive o banco)
```

---

Pronto! Seu ambiente estará funcional em poucos minutos, com execução reproduzível via Docker e autenticação integrada. Bom trabalho!
