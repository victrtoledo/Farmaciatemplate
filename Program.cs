using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Servicios -------------------
// Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=FarmaciaDB.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200", // Angular dev
            "https://farmaciamonteagudo-dhe7dqachzb0f2h4.spaincentral-01.azurewebsites.net") // Angular prod
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ------------------- Middlewares -------------------

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

// Servir Angular
app.UseDefaultFiles();   // index.html automáticamente
app.UseStaticFiles();    // archivos estáticos de Angular

app.UseRouting();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// ------------------- SPA Fallback -------------------
// Cualquier ruta que no sea /api/... sirve index.html
app.MapFallbackToFile("index.html");

// ------------------- DB Inicial -------------------
using (var scope = app.Services.CreateScope())
{
   var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Crear carpeta /home/data si no existe (Azure)
    var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
    if (connectionStr.Contains("/home/data"))
    {
        var dbDir = "/home/data";
        if (!Directory.Exists(dbDir))
            Directory.CreateDirectory(dbDir);
    }

    db.Database.Migrate();
}

app.Run();
