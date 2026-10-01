using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Reflection;

namespace Common.Logging
{
    public static class Serilogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
           (context, configuration) =>
           {
               Console.WriteLine($"************************** {context.HostingEnvironment.ApplicationName} Serilog configuration setting up *******************");
               Console.WriteLine(context.Configuration["ElasticConfiguration:Uri"]);
               var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

               configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(elasticUri))
                        {
                            IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                            AutoRegisterTemplate = true,
                            NumberOfShards = 2,
                            NumberOfReplicas = 1
                        })
                    .ReadFrom.Configuration(context.Configuration);

               if (context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging())
               {
                   Console.WriteLine("**************<<<<<<<< LOGGER PATH >>>>>>>>*****************");
                   Console.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                   var logDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,"logs");
                   Directory.CreateDirectory(logDirectory);

                   configuration
                   .WriteTo.File(
                       path: Path.Combine(logDirectory,"log.txt"),
                       restrictedToMinimumLevel: LogEventLevel.Information,
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true,
                       fileSizeLimitBytes: 10 * 1024 * 1024,
                       flushToDiskInterval: TimeSpan.FromSeconds(1))
                   .WriteTo.Console(new RenderedCompactJsonFormatter());
               }
           };
    }
}