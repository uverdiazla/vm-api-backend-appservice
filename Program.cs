using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text;
using vm_api_backend_appservice.Data;
using vm_api_backend_appservice.Middleware;
using vm_api_backend_appservice.Repositories;
using vm_api_backend_appservice.Services;
using vm_api_backend_appservice.Utils;

Console.WriteLine("Starting VM API " + DateTime.Now.ToString());

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine($"[DEBUG] Connection String: {connectionString}");

// Add DbContext with MySQL
builder.Services.AddDbContext<VmDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVirtualMachineRepository, VirtualMachineRepository>();

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVirtualMachineService, VirtualMachineService>();

// Add JWT Utils
builder.Services.AddSingleton<JwtUtils>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "VM API", 
        Version = "v1", 
        Description = "API for managing virtual machines",
        Contact = new OpenApiContact
        {
            Name = "VM API Team",
            Email = "support@vmapi.com"
        }
    });

    // Enable XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    // Try to include XML comments if file exists (will be generated with XML documentation enabled)
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configure Swagger to use JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

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

    // Add operation filter to document which endpoints require authentication
    c.OperationFilter<SwaggerAuthorizationOperationFilter>();
    
    // Use fully qualified names to avoid ID collisions
    c.CustomSchemaIds(type => type.ToString());
    
    // Avoid duplicate schemas that can cause issues
    c.UseInlineDefinitionsForEnums();
    
    // Configure to handle nullable and optional types
    c.SupportNonNullableReferenceTypes();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true) // Permite cualquier origen
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VM API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at root
    c.DocumentTitle = "VM Management API";
    c.DocExpansion(DocExpansion.List);
});

// Apply database migrations
try
{
    Console.WriteLine("[DEBUG] Applying database migrations...");
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<VmDbContext>();
        dbContext.Database.Migrate();
    }
    Console.WriteLine("[DEBUG] Migrations applied successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] Migration error: {ex.Message}");
    Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Add custom middleware for exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add JWT authentication middleware
app.UseMiddleware<JwtAuthMiddleware>();

// Add role validation middleware
app.UseMiddleware<RoleValidationMiddleware>();

app.MapControllers();

app.Run();
