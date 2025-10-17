using AdventureWorks.Enterprise.Api.Services;
using AdventureWorks.Enterprise.Api.Data;
using AdventureWorks.Enterprise.Api.Middleware;     
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddScoped<AdventureWorks.Enterprise.Api.Services.EmpleadoService>();
builder.Services.AddScoped<AdventureWorks.Enterprise.Api.Services.ProductoInventarioService>();
builder.Services.AddScoped<AdventureWorks.Enterprise.Api.Services.ProductoService>();

// Registra AdventureWorksDbContext para usar SQL Server con la cadena de conexión 'DefaultConnection'
builder.Services.AddDbContext<AdventureWorksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Agrega Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura el pipeline de manejo de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Habilita Swagger en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agrega el middleware de autenticación por API Key
app.UseApiKeyAuth();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
