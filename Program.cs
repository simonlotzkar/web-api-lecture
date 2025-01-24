using Microsoft.EntityFrameworkCore;
using StudentsMinimalApi.Data;
using StudentsMinimalApi.Models;
using StudentsMinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<SchoolDbContext>(
    option => option.UseSqlite(connStr)
);

// Add Cors
builder.Services.AddCors(o => o.AddPolicy("Policy", builder =>
{
    builder.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("Policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My WebAPI");
    });
}

var route = app.MapGroup("/api/students");

route.MapGet("/", StudentService.GetAllStudents);
route.MapGet("/school/{school}", StudentService.GetStudentsBySchool);
route.MapGet("/{id}", StudentService.GetStudent);
route.MapPost("/", StudentService.CreateSttudent);
route.MapPut("/{id}", StudentService.UpdateStudent);
route.MapDelete("/{id}", StudentService.DeleteStudent);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SchoolDbContext>();
    context.Database.Migrate();
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
