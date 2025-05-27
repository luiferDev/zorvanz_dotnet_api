using Microsoft.EntityFrameworkCore;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cargar conexión a base de datos
builder.Services.AddDbContext<ZorvanzContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZorvanzContext")
                         ?? throw new InvalidOperationException("Connection string 'ZorvanzContext' not found."));
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://zorvanz.vercel.app") // Especificar dominios permitidos
            .AllowAnyHeader() // Permitir cualquier encabezado
            .AllowAnyMethod() // Permitir cualquier método HTTP (GET, POST, etc.)
            .AllowCredentials(); // Permitir cookies o credenciales
    });

    /*options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permitir cualquier dominio
            .AllowAnyHeader()
            .AllowAnyMethod();
    });*/
});

// En Program.cs
builder.Services.AddScoped<IProductService, ProductServices>();


var app = builder.Build();

// Aplicar CORS (antes de los controladores)
app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Energías Renovables v1");
        c.RoutePrefix = string.Empty; // Para que Swagger esté en la raíz del proyecto
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();