using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlpacaHerder.Client {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            ConfigureServices(builder.Services, builder.HostEnvironment);
                        
            await builder.Build().RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment env) {

            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(env.BaseAddress) });

        }
    }
}