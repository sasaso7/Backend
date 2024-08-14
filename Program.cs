using BankBackend.Helpers;
using BankBackend.Services;
using EFGetStarted;
using EFGetStarted.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app, app.Environment);

ConfigureEndpoints(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddEndpointsApiExplorer();
    services.AddControllers();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
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
    });

    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IActivityService, ActivityService>();

    services.AddAuthorization(options =>
    {
        var policy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme)
         .RequireAuthenticatedUser()
         .Build();
        options.DefaultPolicy = policy;

        var adminPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme).RequireRole("Admin").Build();
        options.AddPolicy("AdminPolicy", adminPolicy);
    });

    services.AddHttpClient();

    services.AddAuthentication(options =>
        options.DefaultScheme = IdentityConstants.ApplicationScheme)
        .AddCookie(IdentityConstants.ApplicationScheme)
        .AddBearerToken(IdentityConstants.BearerScheme);

    services.ConfigureApplicationCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            },
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

    services.AddHttpContextAccessor();

    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("Database"));
    });

    services.AddIdentityCore<User>(config =>
    {
        config.SignIn.RequireConfirmedEmail = false;
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequiredLength = 6;
        config.Password.RequiredUniqueChars = 1;
    })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddApiEndpoints();

    services.AddTransient<IEmailSender<User>, DummyEmailSender>();

    services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>()
        .AddCheck("APIHealth", () => HealthCheckResult.Healthy());
}

void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API V1");
    });

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
            SeedData.Initialize(scope.ServiceProvider).Wait();
        }
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<CustomAuthorizationMiddleware>();
}

void ConfigureEndpoints(WebApplication app)
{
    app.MapIdentityApi<User>();
    app.MapControllers();

    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                Status = report.Status.ToString(),
                HealthChecks = report.Entries.Select(e => new
                {
                    Component = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description
                }),
                HealthCheckDuration = report.TotalDuration
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    });
}