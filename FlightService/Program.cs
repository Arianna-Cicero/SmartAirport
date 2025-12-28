using FlightService.Data;
using FlightService.Middleware;
using FlightService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Asp.Versioning;
using Amazon.SecretsManager;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/flightservice-.txt", 
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

try
{
    Log.Information("Starting FlightService API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add AWS Services
    builder.Services.AddAWSService<IAmazonSecretsManager>();
    builder.Services.AddScoped<IAwsSecretsManagerService, AwsSecretsManagerService>();

    // Add services to the container
    builder.Services.AddControllers();

    // API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "FlightService API", Version = "v1" });
        
        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Get database credentials from AWS Secrets Manager
    string connectionString;
    var useSecretsManager = builder.Configuration.GetValue<bool>("AWS:UseSecretsManager");
    
    if (useSecretsManager)
    {
        Log.Information("Retrieving database credentials from secure vault");
        var secretName = builder.Configuration["AWS:SecretName"] ?? "";
        
        // Build a temporary service provider to get the secrets manager service
        var tempServiceProvider = builder.Services.BuildServiceProvider();
        var secretsService = tempServiceProvider.GetRequiredService<IAwsSecretsManagerService>();
        
        var credentials = await secretsService.GetDatabaseCredentialsAsync(secretName);
        
        if (credentials != null)
        {
            // If host/port/dbname are not in the secret, get them from configuration
            var host = !string.IsNullOrEmpty(credentials.host) 
                ? credentials.host 
                : builder.Configuration["AWS:DatabaseHost"];
            var port = credentials.port > 0 
                ? credentials.port 
                : builder.Configuration.GetValue<int>("AWS:DatabasePort", 5432);
            var dbname = !string.IsNullOrEmpty(credentials.dbname) 
                ? credentials.dbname 
                : builder.Configuration["AWS:DatabaseName"] ?? "postgres";
            
            connectionString = $"Host={host};Port={port};Database={dbname};Username={credentials.username};Password={credentials.password};SSL Mode=Require;Trust Server Certificate=true;";
            Log.Information("Successfully retrieved database credentials");
        }
        else
        {
            Log.Warning("Failed to retrieve credentials from secure vault, using fallback configuration");
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Database connection string not found");
        }
    }
    else
    {
        Log.Information("Using database connection string from configuration");
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Database connection string not found");
    }

    // DbContext - PostgreSQL
    builder.Services.AddDbContext<FlightServiceDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        // Only enable sensitive logging in Development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<FlightServiceDbContext>("database");

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? "";
    
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
            ValidIssuer = jwtSettings["Issuer"] ?? "FlightService",
            ValidAudience = jwtSettings["Audience"] ?? "FlightServiceClients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

    builder.Services.AddAuthorization();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });

    builder.Services.AddScoped<ITokenService, TokenService>();

    var app = builder.Build();

    // Seed database with initial staff
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<FlightServiceDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            await DbInitializer.SeedData(context, logger);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database");
        }
    }

    // Global Exception Handling Middleware
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();

    // Health Check endpoint
    app.MapHealthChecks("/health");
    app.MapControllers();

    Log.Information("FlightService API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FlightService API failed to start");
}
finally
{
    Log.CloseAndFlush();
}

