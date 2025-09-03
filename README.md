# Formulário de Inscrição (.NET 8 + Blazor Server + SQL Server)

Projeto de exemplo utilizando **.NET 8**, **Blazor Server** (Razor Components), **MudBlazor** e **Entity Framework Core** (SQL Server).
Serve como base para um CRUD simples de inscrições com foco em boas práticas e estrutura mínima para entrevistas e portfólio.

## 🚀 Stack
- .NET 8 / ASP.NET Core
- Blazor Server (Razor Components)
- MudBlazor (UI)
- Entity Framework Core 9 (SQL Server)
- C# 12

## ✨ Funcionalidades
- Página **/enroll**: formulário para criar inscrições.
- Página **/enrollments**: listagem das inscrições já cadastradas.
- Entidade `Enrollment`: `Id`, `FullName`, `Email`, `Course`, `CreatedAt`.
- Persistência em SQL Server via EF Core.

## 📦 Pré‑requisitos
- .NET 8 SDK
- SQL Server (LocalDB, instalação local ou Docker)
- (Opcional) Docker Desktop para subir o SQL Server rapidamente

### Subindo SQL Server via Docker (opcional)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SenhaForte123!" \
  -p 1433:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest
```

## ⚙️ Configuração
Edite `appsettings.json` com sua string de conexão (exemplo):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CourseEnroll;User ID=sa;Password=SenhaForte123!;TrustServerCertificate=True"
  }
}
```

## 🧱 Migrações / Banco de Dados
Criar a base e aplicar migrações (na pasta `src/WebApp`):
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
> O projeto já inclui migrações; rode apenas o `database update` se não quiser recriá-las.

## ▶️ Executando
```bash
dotnet build
dotnet run
# Abra: http://localhost:5090
```

## 🗂️ Estrutura do projeto (resumo)
```
src/WebApp
  ├── Components
  │   ├── Layout
  │   │   ├── MainLayout.razor
  │   │   └── NavMenu.razor
  │   ├── Pages
  │   │   ├── Enroll.razor
  │   │   ├── Enrollments.razor
  │   │   ├── Home.razor
  │   │   └── Counter.razor / Weather.razor (exemplos)
  │   ├── App.razor
  │   └── Routes.razor
  ├── Data
  │   ├── AppDbContext.cs
  │   └── Enrollment.cs
  ├── Migrations
  ├── Program.cs
  ├── appsettings*.json
  └── wwwroot
```

## 🔧 Dicas & Notas
- Se aparecer aviso de **HTTPS redirection** no console em desenvolvimento, é seguro ignorar quando rodando apenas em HTTP.
- Em cenários com **MudBlazor**, os *providers* comuns em layout são:
  ```razor
  <MudThemeProvider />
  <MudPopoverProvider />
  <MudDialogProvider />
  <MudSnackbarProvider />
  ```
  Certifique-se de que **existe apenas um** `<MudPopoverProvider />` por layout e evite saídas duplicadas para overlays/popovers.
- Logs de aviso do MudBlazor (analyzers) em **Desenvolvimento** não impedem a execução.

## 📝 Licença
MIT. Sinta-se à vontade para usar como base em estudos, entrevistas e POCs.