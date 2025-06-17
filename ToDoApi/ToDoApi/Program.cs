using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ToDoApi.Data;


var builder = WebApplication.CreateBuilder(args);

// Configura servicios
builder.Services.AddAuthorization(); // Solo esta línea se queda
builder.Services.AddControllers();

// Agregar EF Core e Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ToDoDb"));
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            )
        };

        // Agrega esto para detectar errores de autenticación
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Token inválido: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Evita redirección al login cuando falla el token
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"No autorizado o token inválido.\"}");
            }
        };
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Coloca el token aquí"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[] {}
        }
    });
});

// Agrega servicios de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();
// Habilita la política de CORS
app.UseCors("AllowFrontend");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();  // ← Esto activa el middleware
app.UseAuthorization();   // ← Esto hace que [Authorize] funcione

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var user = await userManager.FindByEmailAsync("admin@test.com");
    if (user == null)
    {
        var newUser = new IdentityUser { UserName = "admin@test.com", Email = "admin@test.com" };
        await userManager.CreateAsync(newUser, "Admin123*");
    }
}

app.Run();
