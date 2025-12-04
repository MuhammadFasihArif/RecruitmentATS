using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using RecruitmentATS.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(RecruitmentATS.Application.Mappings.MappingProfile));

// Add Application Services
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IJobService, RecruitmentATS.Infrastructure.Services.JobService>();
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IApplicationService, RecruitmentATS.Infrastructure.Services.ApplicationService>();
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IFileService, RecruitmentATS.Infrastructure.Services.LocalFileService>();
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IResumeParser, RecruitmentATS.Infrastructure.Services.ResumeParser>();
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IEmailService, RecruitmentATS.Infrastructure.Services.FileEmailService>();
builder.Services.AddScoped<RecruitmentATS.Application.Interfaces.IAtsMatchingService, RecruitmentATS.Infrastructure.Services.AtsMatchingService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RecruitmentATS.Application.Validators.CreateJobRequestValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RecruitmentATS.Infrastructure.Persistence.DbSeeder.SeedAsync(services, app.Configuration);
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
