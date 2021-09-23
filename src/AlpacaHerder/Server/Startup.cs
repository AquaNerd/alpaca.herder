using Alpaca.Markets;
using AlpacaHerder.Server.Configuration;
using AlpacaHerder.Server.Hubs;
using AlpacaHerder.Server.Services;
using AlpacaHerder.Server.Workers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace AlpacaHerder.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            services
                .AddSignalR()
                .AddHubOptions<MarketDataHub>(options => { 
                    options.EnableDetailedErrors = true; 
                });

            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            
            services.AddOptions();
            services.Configure<AlpacaConfig>(Configuration.GetSection(nameof(AlpacaConfig)));

            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddControllersWithViews();
            services
               .AddRazorPages()
               .AddRazorRuntimeCompilation();

            services.AddSingleton<IMarketDataHub, MarketDataHub>();
            services.AddSingleton<IAlpacaDataStreamingClient>(sp => {
                var config = sp.GetService<IOptions<AlpacaConfig>>();
                return Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(
                    new SecretKey(config.Value.ApiKey, config.Value.ApiSecret));
            });
            services.AddSingleton<IStreamingDataService, StreamingDataService>();

            services.AddHostedService<MarketDataStreamer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseResponseCompression();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<MarketDataHub>("/marketdatahub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
