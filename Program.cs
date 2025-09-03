using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (cookie) + UI padrão (Razor Class Library)
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        // Ajustes mínimos para dev/demo (refine depois)
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// UI do Identity e suportes de página
builder.Services.AddRazorPages();

// Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection(); // só fora de Desenvolvimento
}

app.UseStaticFiles();

// AuthN/AuthZ
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Páginas do Identity (ex.: /Identity/Account/Login)
app.MapRazorPages();

// Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();