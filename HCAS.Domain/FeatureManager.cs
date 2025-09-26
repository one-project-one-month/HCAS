using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Doctors;
using HCAS.Domain.Features.DoctorSchedule;
using HCAS.Domain.Features.Patient;
using HCAS.Domain.Features.Specialization;
using HCAS.Domain.Features.Staff;
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
            builder.Services.AddDbContext<AppDbContext>(opt =>
            { 
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));

            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            // Register services
            builder.Services.AddTransient<DapperService>();
            builder.Services.AddScoped<DoctorService>();
            builder.Services.AddScoped<DoctorScheduleService>();
            builder.Services.AddScoped<SpecializationSerivce>();
            builder.Services.AddScoped<StaffService>();
            builder.Services.AddScoped<PatientService>();
        }
    }
}
