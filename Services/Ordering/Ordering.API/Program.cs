using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ordering.API;
using Ordering.Application.Handlers;
using Ordering.Application.Mapper;
using Ordering.Core.IRepositories;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Repositories;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure centralized Serilog logging
builder.Host.UseSerilog(Serilogger.Configure);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
/**** Healthcheck for DB (SQLSERVER)******/
builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
sqlServerOptionsAction: sqlOptions => {
    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
    sqlOptions.MigrationsAssembly("Ordering.API");
}));
builder.Services.AddHealthChecks().AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), name: "SQL Server health check", failureStatus: HealthStatus.Degraded);
builder.Services.AddResponseCaching();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? []);
    });
});

var assemblies = new Assembly[]
{
    typeof(Program).Assembly,
    typeof(CreateOrderCommandHandler).Assembly
};
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(OrderMappingProfile).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
/** Register services **/
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();// Serilog HTTP request logging
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
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
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
    await context.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(context);
}
app.Run();
