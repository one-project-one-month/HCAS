using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Doctors;
using HCAS.Domain.Features.DoctorSchedule;
using HCAS.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HCAS.Domain
{
    public static class FeatureManager
    {
        public static void AddDomain(this WebApplicationBuilder builder)
        {
            // Configure DbContext with retry-on-failure
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DbConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,              
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null        
                        );
                    });
            });

            // Register services
            builder.Services.AddTransient<DapperService>();
            builder.Services.AddScoped<DoctorService>();
            builder.Services.AddScoped<DoctorScheduleService>();
            builder.Services.AddScoped<Specialization>();
            builder.Services.AddScoped<Staff>();
        }
    }

}
