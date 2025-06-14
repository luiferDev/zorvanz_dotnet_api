using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configurar servicios de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Zorvanz",
        Version = "v1",
        Description = "API para gestionar usuarios con autenticación JWT."
    });

    // Configurar el esquema de autenticación Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en este formato: Bearer {token}"
    });

    // Requerir el esquema en todos los endpoints protegidos
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Configuration.AddEnvironmentVariables();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
    throw new InvalidOperationException("La variable de entorno 'DB_CONNECTION_STRING' no está definida o está vacía.");

// Cargar conexión a base de datos
builder.Services.AddDbContext<ZorvanzContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configurar JWT Authentication
// var jwtSettings = builder.Configuration.GetSection("Jwt");
// var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException());

// Obtener la key desde la variable de entorno
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
             throw new InvalidOperationException("JWT_KEY no está configurada en las variables de entorno");
var key = Encoding.UTF8.GetBytes(jwtKey);



builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    var signingkey = new SymmetricSecurityKey(key);
    var signingCredential = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256Signature);

    opt.RequireHttpsMetadata = false;

    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signingkey
    };
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", 
                "http://localhost:5173/login", 
                "https://zorvanz.vercel.app",
                "https://zorvanz.vercel.app/login") // Especificar dominios permitidos
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UpdatePasswordService>();


var app = builder.Build();

// Aplicar CORS (antes de los controladores)
app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Zorvanz v1");
        c.RoutePrefix = string.Empty; // Para que Swagger esté en la raíz del proyecto
    });
}

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();