# Guia de criação do ambiente  
_Formulário de Inscrição (.NET 8 + Blazor Server + SQL Server + MudBlazor)_

> Este guia usa **Git Bash** no Windows, mas os comandos funcionam (salvo diferenças) em macOS/Linux.

---

## 1) Pré-requisitos

- **Git** + **Git Bash**
- **.NET SDK 8.0.x**
  - Verifique: `dotnet --version` (deve começar com `8.`)
- **SQL Server 2019/2022** (local) **ou** Docker para subir um container do SQL Server
- (Opcional) **VS Code** com _C# Dev Kit_ e _C# Extensions_

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

## 3) Restaurar dependências

```bash
dotnet restore
```

---

## 4) Subir o SQL Server (opção A — Docker)

> Pule para a opção B se você já tem SQL Server local.

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SenhaForte123!"   -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest
```

- A senha do usuário `sa` no exemplo é **SenhaForte123!** (ajuste se quiser).
- Aguarde ~10–20s após o `docker run` até o SQL estar pronto.

### Opção B — SQL Server instalado na máquina

- Garanta que o serviço esteja em execução e aceite conexões na porta **1433**.
- Tenha um usuário com permissão para criar banco (ex.: `sa`).

---

## 5) Configurar a _connection string_

O repositório já traz `src/WebApp/appsettings.json` **sanitizado**.  
Para desenvolvimento local, **não** altere esse arquivo. Crie **appsettings.Development.json** (está no `.gitignore`) para sua máquina:

```bash
# ainda em src/WebApp
cat > appsettings.Development.json << 'JSON'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CourseEnroll;User ID=sa;Password=SenhaForte123!;TrustServerCertificate=True"
  }
}
JSON
```

> Se você trocou a senha/porta ou usa uma instância nomeada, ajuste a string.

---

## 6) Aplicar as migrações (criar o banco)

Instale a CLI do EF Core (se ainda não tiver):

```bash
dotnet tool install -g dotnet-ef
# se já tiver: dotnet tool update -g dotnet-ef
```

> Garanta que `~/.dotnet/tools` está no seu `PATH`.  
> No Git Bash, normalmente já está. Se precisar:
> ```bash
> export PATH="$PATH:$HOME/.dotnet/tools"
> ```

Aplique as migrações:

```bash
cd ~/dev/formulario-inscricao/src/WebApp
dotnet ef database update
```

Isso criará o banco **CourseEnroll** e as tabelas (baseado em `Migrations/`).

---

## 7) Executar a aplicação

```bash
cd ~/dev/formulario-inscricao/src/WebApp
dotnet build
dotnet run
```

Saída esperada no console (exemplo):

```
Now listening on: http://localhost:5090
Application started. Press Ctrl+C to shut down.
```

Acesse no navegador:

- **http://localhost:5090**
- **http://localhost:5090/enroll** — formulário de inscrição
- **http://localhost:5090/enrollments** — últimas inscrições

---

## 8) Teste rápido

1. Abra **/enroll**, preencha:
   - Nome completo, E-mail e (opcional) Curso.
2. Envie.
3. Acesse **/enrollments** e confirme que o registro foi salvo.

---

## 9) Dicas & avisos comuns

### 🔹 Aviso de HTTPS Redirection
Em desenvolvimento você pode ver:
```
warn: ...HttpsRedirectionMiddleware[...] Failed to determine the https port for redirect.
```
É seguro ignorar quando rodando apenas em HTTP. Se quiser suprimir:
- **Opção simples:** comente `app.UseHttpsRedirection();` em `Program.cs` durante o dev.
- **Ou:** configure HTTPS/launchSettings com portas válidas.

### 🔹 MudBlazor — Providers
O layout deve conter **um** set de providers no topo e **nenhum `SectionOutlet` manual**:

`Components/Layout/MainLayout.razor`
```razor
@inherits LayoutComponentBase
@using MudBlazor

<MudThemeProvider />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<div class="page">
  <!-- seu layout -->
  @Body
</div>
```

**Não** use `<SectionOutlet SectionName="mud-overlay-to-popover-provider" />`.  
Se um dia aparecer:
- `Missing <MudPopoverProvider />` → faltou `<MudPopoverProvider />` no layout.
- `There is already a subscriber ... 'mud-overlay-to-popover-provider'` → havia `SectionOutlet` duplicado; remova qualquer `<SectionOutlet ...>`.

Para checar rapidamente:

```bash
# dentro de src/WebApp
grep -Rn --include="*.razor" -E "<SectionOutlet\b|MudPopoverProvider" .
```

### 🔹 EF CLI não encontrada
Adicione o caminho das ferramentas ao `PATH`:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

### 🔹 Conflito de porta
Se 5090 estiver ocupado, rode:
```bash
dotnet run --urls http://localhost:5095
```

### 🔹 Aviso CRLF/LF do Git
Seguro ignorar; ou configure:
```bash
git config core.autocrlf true
```

---

## 10) Estrutura do projeto (resumo)

```
src/WebApp
├── Components
│   ├── Layout
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages
│   │   ├── Enroll.razor
│   │   ├── Enrollments.razor
│   │   ├── Home.razor / Counter.razor / Weather.razor
│   ├── App.razor
│   └── Routes.razor
├── Data
│   ├── AppDbContext.cs
│   └── Enrollment.cs
├── Migrations
├── wwwroot (css, favicon, bootstrap)
├── Program.cs
├── WebApp.csproj / WebApp.sln
├── appsettings.json                 (sanitizado)
└── appsettings.Development.json     (local, no .gitignore)
```

---

## 11) Parar/limpar

- Parar a aplicação: **Ctrl+C** no terminal.
- SQL Server (Docker):
  ```bash
  docker stop mssql
  docker rm mssql
  ```
- Remover banco (local): use SSMS/Azure Data Studio ou `DROP DATABASE CourseEnroll;`.

---

## 12) Próximos passos (opcional)

- **Publicar** para produção:
  ```bash
  dotnet publish -c Release -o out
  ```
  Depois execute o binário em um servidor (Kestrel/Reverse Proxy).

- **CI/CD**: adicionar GitHub Actions (_.NET Desktop_ / _ASP.NET Core build & test_).

---

Se algo não bater com seu ambiente, me avise qual passo e mensagem de erro que eu te ajudo a ajustar rapidamente.
