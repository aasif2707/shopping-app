using Catalog.API;
using Catalog.Application.Handlers;
using Catalog.Application.Mapper;
using Catalog.Core.Repositories;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Repositories;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure centralized Serilog logging
builder.Host.UseSerilog(Serilogger.Configure);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
/**** Healthcheck for DB (MongoDB)******/
builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration["DatabaseSettings:ConnectionString"]));
builder.Services.AddHealthChecks().AddMongoDb(builder.Configuration["DatabaseSettings:ConnectionString"], "Catalog  MongoDb Health Check", HealthStatus.Degraded);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assemblies = new Assembly[]
{
    typeof(Program).Assembly,
    typeof(GetAllBrandsQueryHandler).Assembly
};
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, CatalogRepository>();
builder.Services.AddScoped<ITypeRepository, CatalogRepository>();
builder.Services.AddScoped<IBrandRepository, CatalogRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? []);
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", limiter =>
    {
        limiter.PermitLimit = 100;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});
//Reverse-proxy safe hosting(deployment)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Clear the default known networks to allow all
    options.KnownProxies.Clear();  // Clear the default known proxies to allow all
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();// Serilog HTTP request logging
app.UseSwagger();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1"));
app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseCors("frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
