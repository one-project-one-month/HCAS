using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Appoinment;
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
            builder.Services.AddTransient<DoctorService>();
            builder.Services.AddTransient<DoctorScheduleService>();
            builder.Services.AddTransient<SpecializationService>();
            builder.Services.AddTransient<StaffService>();
            builder.Services.AddTransient<AppointmentService>();
            builder.Services.AddTransient<PatientService>();
        }
    }
}
