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
using vm_api_backend_appservice.Hubs;

Console.WriteLine("Starting VM API " + DateTime.Now.ToString());

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Configure SignalR
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true; // Enable detailed errors for debugging
    options.MaximumReceiveMessageSize = 102400; // Set message size limit (100 KB)
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
builder.Services.AddScoped<IVirtualMachineNotificationService, VirtualMachineNotificationService>();

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
    
    // Configure JWT Bearer Auth to play nice with SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            
            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && 
                path.StartsWithSegments("/hubs/virtualmachines"))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true) // Allow any origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Required for SignalR
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

// Use CORS - must be placed before SignalR endpoints
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Add custom middleware for exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add JWT authentication middleware
app.UseMiddleware<JwtAuthMiddleware>();

// Add role validation middleware
app.UseMiddleware<RoleValidationMiddleware>();

app.MapControllers();

// Configure SignalR routes - send detailed client errors back for debugging
app.MapHub<VirtualMachineHub>("/hubs/virtualmachines", options => {
    options.CloseOnAuthenticationExpiration = false; // Don't close connection when auth expires
    options.TransportMaxBufferSize = 102400; // 100 KB
    options.ApplicationMaxBufferSize = 102400; // 100 KB
    options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(30);
});

app.Run();
