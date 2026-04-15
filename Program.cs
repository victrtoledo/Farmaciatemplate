using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// -----------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=FarmaciaDB.db"));

// CONFIGURACIÓN DE CORS: Definir la política
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200", // Angular en desarrollo
                "https://taller-fcfhfmhkhubda0d2.spaincentral-01.azurewebsites.net" // Angular en producción
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Opcional: permitir cookies/auth si fuera necesario
    });
});

var app = builder.Build();

// -----------------------------------------------------------
// 2. CONFIGURACIÓN DEL PIPELINE (Middlewares - EL ORDEN IMPORTA)
// -----------------------------------------------------------

// A. Siempre lo primero: CORS 
// Debe ir antes de Routing, Auth y StaticFiles para responder a las peticiones 'OPTIONS'
app.UseCors("AllowAngular");

// B. Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// C. Seguridad y Redirección
// Si usas HTTP en local, asegúrate de que el frontend apunte al puerto correcto
app.UseHttpsRedirection();

// D. Archivos Estáticos (Angular)
app.UseDefaultFiles();   // Busca index.html por defecto
app.UseStaticFiles();    // Sirve los archivos de la carpeta wwwroot

// E. Rutas y Autorización
app.UseRouting();
app.UseAuthorization();

// F. Mapeo de Endpoints
app.MapControllers();

// G. SPA Fallback
// Si la ruta no es un archivo ni un endpoint de API, sirve el index de Angular
app.MapFallbackToFile("index.html");

// -----------------------------------------------------------
// 3. INICIALIZACIÓN DE BASE DE DATOS
// -----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Crea la DB si no existe
}

app.Run();