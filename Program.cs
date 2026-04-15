using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 1. CONFIGURACIÓN DE SERVICIOS
// -----------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyección del DbContext leyendo la ruta del appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200", 
                "https://farmaciamonteagudo-dhe7dqachzb0f2h4.spaincentral-01.azurewebsites.net"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// -----------------------------------------------------------
// 2. PIPELINE DE MIDDLEWARES (EL ORDEN ES VITAL)
// -----------------------------------------------------------

// Swagger siempre disponible para que puedas usar Postman fácilmente en producción si lo necesitas
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Servir archivos de Angular (deben estar en la carpeta wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting(); // <--- 1º Routing

app.UseCors("AllowAngular"); // <--- 2º CORS (Siempre después de Routing)

app.UseAuthorization(); // <--- 3º Autorización

app.MapControllers();

// Fallback para Angular: Si la ruta no es de la API, devuelve el index.html
app.MapFallbackToFile("index.html");

// -----------------------------------------------------------
// 3. INICIALIZACIÓN AUTOMÁTICA DE LA DB
// -----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var db = services.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear la base de datos.");
    }
}

app.Run();