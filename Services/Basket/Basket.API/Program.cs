using Basket.API;
using Basket.Application.GrpcService;
using Basket.Application.Handlers;
using Basket.Application.Mapper;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Discount.Application.Protos.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MassTransit;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure centralized Serilog logging
builder.Host.UseSerilog(Serilogger.Configure);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
var assemblies = new Assembly[]
{
    typeof(Program).Assembly,
    typeof(CreateShoppingCartCommandHandler).Assembly
};
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BasketMappingProfile).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

/**** Healthcheck for DB (Redis)******/
builder.Services.AddHealthChecks().AddRedis(builder.Configuration["CacheSettings:ConnectionString"], "Redis Health", HealthStatus.Degraded);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtosService.DiscountProtosServiceClient>(o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? []);
    });
});
//Reverse-proxy safe hosting(deployment)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Clear the default known networks to allow all
    options.KnownProxies.Clear();  // Clear the default known proxies to allow all
});
/**********RabbitMQ EventBus (Producer) ***********/
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();// Serilog HTTP request logging
app.UseSwagger();
app.UseSwaggerUI();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseResponseCaching();
app.UseCors("frontend");
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
