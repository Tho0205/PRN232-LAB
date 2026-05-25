using Microsoft.EntityFrameworkCore;
using PRN232.LAB1.Repositories;
using PRN232.LAB1.Repositories.Implementations;
using PRN232.LAB1.Repositories.Interfaces;
using PRN232.LAB1.Services.Implementations;
using PRN232.LAB1.Services.Interfaces;
using PRN232.LAB1.Services.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<PRN232.LAB1.API.Mappers.ApiMappingProfile>();
    cfg.AddProfile<PRN232.LAB1.Services.Mappers.ServiceMappingProfile>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LAB1 API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    dbContext.Database.EnsureCreated(); 
}

app.Run();
