using Microsoft.EntityFrameworkCore;
using zorvanz_api.Services;
using zorvanz_api.ZorvanzDbContext;

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

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException());

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