using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 1. CONFIGURACIÓN DE SERVICIOS
// -----------------------------------------------------------

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = 
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB - Lee del appsettings o usa el fallback local
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=FarmaciaDB.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

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

// Swagger disponible para usar Postman fácilmente
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Servir archivos de Angular (deben estar en la carpeta wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting(); // <--- 1º Routing

app.UseCors("AllowAngular"); // <--- 2º CORS

app.UseAuthorization(); // <--- 3º Autorización (aunque no tengas JWT, se deja por estructura)

app.MapControllers();

// Fallback para Angular
app.MapFallbackToFile("index.html");

// -----------------------------------------------------------
// 3. INICIALIZACIÓN DE LA DB Y CARPETAS
// -----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var db = services.GetRequiredService<AppDbContext>();

        // Lógica que copiaste de tu otra web: Crear carpeta /home/data si no existe (Para Azure)
        var currentConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
        if (currentConn.Contains("/home/data"))
        {
            var dbDir = "/home/data";
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
        }

        // Crea la base de datos si no existe
        db.Database.EnsureCreated(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear la base de datos.");
    }
}

app.Run();