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
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            //dependency injections
            builder.Services.AddTransient<DapperService>();
            builder.Services.AddTransient<DoctorService>();
            builder.Services.AddTransient<DoctorScheduleService>();


        }
    }

}
