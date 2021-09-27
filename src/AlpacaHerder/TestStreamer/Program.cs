using Alpaca.Markets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestStreamer.Configuration;

namespace TestStreamer {
    internal class Program {
        private static ILogger _logger;

        internal static int Main(string[] args) {
            var services = new ServiceCollection();
            
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            try {
                _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

                var service = serviceProvider.GetService<IStreamingDataService>();
                Task.Run(() => {
                    service.ListenAsync("SPY", default).GetAwaiter().GetResult();
                });

                Console.ReadKey();

                return 0;
            } catch (Exception ex) {
                _logger.LogCritical(ex.ToString());
                Thread.Sleep(1000);
                return 1;
            } finally {
                serviceProvider.DisposeAsync().GetAwaiter().GetResult();
            }
        }

        private static void ConfigureServices(IServiceCollection services) {

            var config = new ConfigurationBuilder()
                                          .AddJsonFile("appsettings.json", true)
                                          .AddJsonFile("appsettings.development.json", true)
                                          .Build();

            services.AddOptions();
            services.Configure<AlpacaConfig>(config.GetSection(nameof(AlpacaConfig)));
            services.AddSingleton<IAlpacaDataStreamingClient>(sp => {
                var clientConfig = sp.GetService<IOptions<AlpacaConfig>>();
                return Alpaca.Markets.Environments.Paper.GetAlpacaDataStreamingClient(
                    new SecretKey(clientConfig.Value.ApiKey, clientConfig.Value.ApiSecret));
            });
            services.AddSingleton<IStreamingDataService, StreamingDataService>();

            services.AddLogging(builder =>
                builder.AddConsole()
                       .AddDebug()
                       .SetMinimumLevel(LogLevel.Trace)
            );
        }
    }
}