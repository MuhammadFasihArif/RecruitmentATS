using RecruitmentATS.WebUI.Components;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentATS.Infrastructure.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddSignInManager()
    .AddDefaultTokenProviders();

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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
