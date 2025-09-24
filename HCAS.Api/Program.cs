using HCAS.Domain;
using HCAS.Domain.Features.Staff;
using HCAS.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register DapperService
builder.Services.AddSingleton<DapperService>();
// Register StaffService
builder.Services.AddTransient<StaffService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddDomain();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
