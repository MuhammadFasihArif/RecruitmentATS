using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentATS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentATS.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Seed roles
        string[] roles = { "Admin", "Recruiter", "Candidate" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin user
        var adminEmail = configuration["DefaultAdmin:Email"] ?? "admin@recruitmentats.com";
        var adminPassword = configuration["DefaultAdmin:Password"] ?? "Admin@123";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Jobs
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await context.Jobs.AnyAsync())
        {
            var jobs = new List<Job>
            {
                new Job
                {
                    Id = Guid.NewGuid(),
                    Title = "Senior Software Engineer",
                    Description = "We are looking for an experienced .NET developer to join our team. You will be working on high-scale web applications.",
                    Location = "Remote",
                    Requirements = "- 5+ years of experience with C# and .NET Core\n- Experience with Azure\n- Strong problem-solving skills",
                    SalaryRangeMin = 120000,
                    SalaryRangeMax = 160000,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Job
                {
                    Id = Guid.NewGuid(),
                    Title = "Product Manager",
                    Description = "Lead the product vision and strategy for our new recruitment platform.",
                    Location = "New York, NY",
                    Requirements = "- 3+ years of product management experience\n- Experience in HR tech is a plus\n- Excellent communication skills",
                    SalaryRangeMin = 100000,
                    SalaryRangeMax = 140000,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Job
                {
                    Id = Guid.NewGuid(),
                    Title = "UX Designer",
                    Description = "Design intuitive and beautiful user interfaces for our web and mobile apps.",
                    Location = "San Francisco, CA",
                    Requirements = "- Portfolio demonstrating strong UX/UI skills\n- Proficiency in Figma\n- Experience with design systems",
                    SalaryRangeMin = 90000,
                    SalaryRangeMax = 130000,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Jobs.AddRangeAsync(jobs);
            await context.SaveChangesAsync();
        }
    }
}
