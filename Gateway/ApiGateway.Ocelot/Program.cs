using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Serilog;
using Common.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});
builder.Services.AddOcelot().AddCacheManager(o => o.WithDictionaryHandle());

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors("CorsPolicy");
app.MapGet("/", () => "Hello Ocelot");// Gateway health/root endpoint
await app.UseOcelot();

app.Run();

