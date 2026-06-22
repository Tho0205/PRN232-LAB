using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using PRN232.LAB.API.Middleware;
using PRN232.LAB.Repositories;
using PRN232.LAB.Repositories.Implementations;
using PRN232.LAB.Repositories.Interfaces;
using PRN232.LAB.Services.Implementations;
using PRN232.LAB.Services.Interfaces;
using PRN232.LAB.Services.Mappers;
using PRN232.LAB.Services.Models.Options;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
        options.RespectBrowserAcceptHeader = true;
    })
    .AddXmlSerializerFormatters()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PRN232.LAB.API.Models.Validators.LoginRequestValidator>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var response = new PRN232.LAB.Services.Models.Responses.ApiResponse<object>
        {
            Success = false,
            Message = "Invalid input data",
            Data = null,
            Errors = errors
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
    };
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<PRN232.LAB.API.Mappers.ApiMappingProfile>();
    cfg.AddProfile<PRN232.LAB.Services.Mappers.ServiceMappingProfile>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PRN232 LAB API",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PRN232 LAB API",
        Version = "v2"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token."
    });

    options.AddSecurityRequirement(_ => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", null, null)] = new List<string>()
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    });
});

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LAB API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "PRN232 LAB API v2");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();

    const int maxAttempts = 30;
    const int delaySeconds = 2;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            dbContext.Database.EnsureCreated();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUnitOfWork>().UserRepository;
            var existingAdmin = await userRepository.GetAllAsync(u => u.Username == "admin");
            if (!existingAdmin.Any())
            {
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<PRN232.LAB.Repositories.Entities.User>();
                var admin = new PRN232.LAB.Repositories.Entities.User
                {
                    Username = "admin",
                    Role = "Admin"
                };
                admin.PasswordHash = hasher.HashPassword(admin, "123456");
                await userRepository.InsertAsync(admin);
                await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveAsync();
            }
            break;
        }
        catch (Exception) when (attempt < maxAttempts)
        {
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

app.Run();
