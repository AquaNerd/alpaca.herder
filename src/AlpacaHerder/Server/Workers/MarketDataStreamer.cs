using AlpacaHerder.Server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Workers {
    public class MarketDataStreamer : IHostedService, IAsyncDisposable {
        private readonly ILogger<MarketDataStreamer> _logger;
        private readonly IStreamingDataService _streamingDataService;

        public MarketDataStreamer(ILogger<MarketDataStreamer> logger, IStreamingDataService streamingDataService) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _streamingDataService = streamingDataService ?? throw new ArgumentNullException(nameof(streamingDataService));
            _logger.LogDebug($"{nameof(MarketDataStreamer)} ctor hit");
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogDebug($"{nameof(MarketDataStreamer)} - {nameof(StartAsync)} Hit");
            await _streamingDataService.ListenAsync("AAPL", cancellationToken);
            
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogDebug($"{nameof(MarketDataStreamer)} - {nameof(StopAsync)} Hit");
            await _streamingDataService.UnListenAsync("AAPL", cancellationToken);
        }

        public async ValueTask DisposeAsync() {
            await _streamingDataService.DisposeAsync();
        }
    }
}