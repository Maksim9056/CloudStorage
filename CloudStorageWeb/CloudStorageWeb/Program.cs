using CloudStorageWeb.Client.Pages;
using CloudStorageWeb.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Serilog;
namespace CloudStorageWeb
{
    public class Program
    {
        static readonly Prometheus.Counter s_hatsSold = Metrics.CreateCounter("hats_sold",
            "The number of hats sold in our store");

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddHttpClient();
            //builder.Services.UseHttpClientMetrics();

            var app = builder.Build();

            //app.
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var log = new LoggerConfiguration().WriteTo
            .WriteTo.Http("http://localhost:8080")
           .CreateLogger();

            // Setup Prometheus metrics endpoint
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/metrics"))
                {
                    await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
                }
                else
                {
                    await next();
                }
            });

            var rand = new Random();
            var timer = new Timer(_ =>
            {
                s_hatsSold.Inc(rand.Next(0, 1000)); // Increment the hats sold counter
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Example: add hats sold every 10 seconds



            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Start the timer

            app.Run();
        }
    }
}
