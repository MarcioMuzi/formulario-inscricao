# SETUP — Formulário de Inscrição (.NET 8 + Blazor Server + SQL Server + Identity + Docker)

Guia de preparação e execução do projeto com **Docker Compose** (recomendado) e **execução local**. Inclui comandos prontos para **Git Bash** e notas de resolução de problemas.

> Projeto: **Blazor Server sem APIs**, autenticação com **ASP.NET Core Identity (cookie)**, UI com **MudBlazor**, persistência via **EF Core** (SQL Server).

---

## ✅ Checklist rápido

1. **.NET 8 SDK** instalado  
2. **Docker Desktop** ativo  
3. **(Opcional) dotnet-ef** instalado/atualizado  
4. **Executar via Docker:** `docker compose build && SA_PASSWORD='Pass@w0rd!' docker compose up -d`  
5. **Aplicar migrations (host → DB do container):**
   ```bash
   ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'      dotnet ef database update
   ```
6. Acessar:  
   - Registro: `http://localhost:5090/Identity/Account/Register`  
   - Login: `http://localhost:5090/Identity/Account/Login`  
   - Formulário: `http://localhost:5090/enroll`  
   - Listagem (protegida): `http://localhost:5090/enrollments`

---

## 🧰 Pré‑requisitos

- **.NET 8 SDK** – https://dotnet.microsoft.com/download  
- **Docker Desktop** (com `docker compose`)  
- **(Opcional) EF Core CLI**  
  ```bash
  dotnet tool update -g dotnet-ef || dotnet tool install -g dotnet-ef
  ```

---

## 🐳 Execução com Docker (recomendado)

### 1) Subir containers
Na raiz `src/WebApp`:
```bash
docker compose build
SA_PASSWORD='Pass@w0rd!' docker compose up -d
docker compose ps
```

Serviços:
- **web** (ASP.NET): `http://localhost:5090`
- **sql** (SQL Server): porta exposta **14333**

> O `docker-compose.yml` já define a connection string do **web** usando o host **sql** (rede interna do Compose).

### 2) Aplicar migrations (do **host** para o banco do container)
```bash
ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'   dotnet ef database update
```

### 3) Testar
- **Registrar usuário**: `/Identity/Account/Register`  
- **Fazer login**: `/Identity/Account/Login`  
- **Criar inscrição**: `/enroll`  
- **Listar inscrições (autenticado)**: `/enrollments`

### 4) Logs e manutenção
```bash
docker compose logs -f web
docker compose logs -f sql
docker compose build web && docker compose up -d web   # rebuild do web
docker compose down -v                                 # derruba e remove volumes (apaga o banco)
```

> **Chaves de DataProtection persistidas**: o serviço **web** monta um volume em `/root/.aspnet/DataProtection-Keys`, evitando warnings/erros de antiforgery após restart.

---

## ▶️ Execução **sem** Docker (opcional)

1) Configure a **connection string** real em `appsettings.Development.json` **ou** via **user-secrets**:
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CourseEnroll;Trusted_Connection=True;TrustServerCertificate=True"
```

2) Aplique migrations e rode:
```bash
dotnet ef database update
dotnet run
# http://localhost:5090
```

> Em `Development` o projeto roda em **HTTP**; HSTS/HTTPS redireciona apenas fora de `Development`.

---

## 🧱 Migrations EF — comandos úteis

Criar uma migration (ex.: novos campos em Enrollment):
```bash
dotnet ef migrations add AddEnrollmentFields -c AppDbContext
```

Aplicar no **banco do container** (porta 14333) a partir do **host**:
```bash
ConnectionStrings__DefaultConnection='Server=localhost,14333;Database=CourseEnroll;User Id=sa;Password=Pass@w0rd!;TrustServerCertificate=True;Encrypt=False'   dotnet ef database update
```

Listar migrations e reverter:
```bash
dotnet ef migrations list
dotnet ef database update NomeDaMigrationAnterior
```

---

## 🧩 MudBlazor — configuração mínima

No `App.razor`, garanta:
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
  <script src="_content/MudBlazor/MudBlazor.min.js"></script>
</body>
</html>
```

No layout (`MainLayout.razor`), **um único** conjunto de providers:
```razor
<MudThemeProvider />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />
```

### Diagnóstico rápido (providers/sections)
```bash
grep -RInE --include='*.razor' -e '<SectionOutlet' -e 'mud-overlay-to-popover-provider' -e '<MudPopoverProvider' .
```

---

## 🔐 Identity (cookie) — resumo

- **Program.cs**: `AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();` + `UseAuthentication(); UseAuthorization(); app.MapRazorPages();`
- **Routes.razor**: `CascadingAuthenticationState` + `AuthorizeRouteView`
- **Páginas**: pode proteger com
  ```razor
  @attribute [Microsoft.AspNetCore.Authorization.Authorize]
  ```
- **Layout da área Identity**: arquivo local `Areas/Identity/Pages/_Layout.cshtml` simplifica a UI padrão (evita depender de `_LoginPartial`).

URLs úteis:
- Registro: `/Identity/Account/Register`
- Login: `/Identity/Account/Login`

> **Não há usuário padrão**: registre um usuário pela própria UI.

---

## 🧯 Troubleshooting

### 1) Antiforgery/DataProtection após reinício
- Sintoma: *“The antiforgery token could not be decrypted”* após restart.  
- Correção: volume `web_keys` já configurado. Se persistir:
  ```bash
  docker compose up -d --force-recreate web
  # e limpe os cookies do site no navegador
  ```

### 2) MudBlazor JSInterop
- Sintoma: *“Could not find 'mudElementRef.addOnBlurEvent'”*.  
- Cheque na aba **Network** se carrega `/_content/MudBlazor/MudBlazor.min.js` (status `200`).  
- Garanta que o `<script src="_content/MudBlazor/MudBlazor.min.js">` está **após** `blazor.web.js`.

### 3) Duplicidade de providers/sections
- Sintoma: *“There is already a subscriber… mud-overlay-to-popover-provider”*.  
- Use o **grep** acima e mantenha **apenas um** `MudPopoverProvider` no layout.

---

## 🔄 Fluxo Git (sugerido)

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feat/sua-feature
# ... commits ...
git push -u origin HEAD
# MR -> develop -> (após homologar) -> main
```

Mensagens de commit curtas e descritivas; MR com contexto, escopo, validação e riscos/rollback.

---

## 📎 Portas & Credenciais

- **Web**: `http://localhost:5090`
- **SQL**: `localhost:14333` (usuário `sa`, senha definida por `SA_PASSWORD`, ex.: `Pass@w0rd!`)
- **Login**: crie via `/Identity/Account/Register`

---

## 📄 Licença

MIT. Use livremente para estudos e entrevistas.
