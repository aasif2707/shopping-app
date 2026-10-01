using Common.Logging;
using Discount.API.Services;
using Discount.Application.Commands;
using Discount.Application.Mapper;
using Discount.Core;
using Discount.Infrastructure;
using Discount.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure centralized Serilog logging
builder.Host.UseSerilog(Serilogger.Configure);

// Add services to the container.
builder.Services.AddControllers();
var assemblies = new Assembly[]
{
    typeof(Program).Assembly,
    typeof(CreateDiscountCommand).Assembly
};
builder.Services.AddMediatR(assemblies);
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DiscountMapper).Assembly));
builder.Services.AddGrpc();
/**** Healthcheck for DB (Postgres)******/
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration["DatabaseSettings:ConnectionString"], failureStatus: HealthStatus.Degraded);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSerilogRequestLogging();// Serilog HTTP request logging
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.MapGrpcService<DiscountService>();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
using (var scope = app.Services.CreateScope())
{
    DatabaseSeeder.Seed(builder.Configuration);
}
app.Run();
