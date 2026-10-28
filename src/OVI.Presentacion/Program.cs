using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Logica.Servicios;
using OVI.Modelos.Entidades;
using OVI.Presentacion.Servicios;
using Microsoft.AspNetCore.Authentication;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("OVIConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión OVIConnection.");
builder.Services.AddDbContext<OVIDbContext>(options => options.UseSqlServer(connection));
builder.Services.AddScoped<ServicioVocacional>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.Configure<ChatbotIAOptions>(builder.Configuration.GetSection("ChatbotIA"));
builder.Services.AddScoped<IChatbotFAQService, ChatbotFAQService>();
builder.Services.AddHttpClient<IChatbotIAService, ChatbotIAService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = TimeSpan.FromMinutes(30);
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/Denegado";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnValidatePrincipal = async context =>
        {
            var idText = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idText, out var id)) { context.RejectPrincipal(); return; }
            var db = context.HttpContext.RequestServices.GetRequiredService<OVIDbContext>();
            var user = await db.Usuarios.Include(x => x.Rol).AsNoTracking().FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (user is null || !user.Estado || user.Rol?.Nombre != context.Principal?.FindFirstValue(ClaimTypes.Role))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Opt-in: Docker usa una base nueva; en desarrollo local las migraciones se ejecutan manualmente.
if (app.Configuration.GetValue<bool>("OVI_APPLY_MIGRATIONS"))
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Migraciones");
    for (int intento = 1; intento <= 12; intento++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<OVIDbContext>().Database.MigrateAsync();
            break;
        }
        catch (Exception ex) when (intento < 12)
        {
            logger.LogWarning(ex, "Base de datos aún no lista; reintento {Intento}/12", intento);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}
await AdminInicializador.CrearSiConfiguradoAsync(app.Services, app.Configuration);
app.Run();
