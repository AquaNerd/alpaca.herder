using AlpacaHerder.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Hubs {
    public class MarketDataHub : Hub, IMarketDataHub {

        private readonly ILogger<MarketDataHub> _logger;
        private readonly IHubContext<MarketDataHub> _hubContext;

        public MarketDataHub(ILogger<MarketDataHub> logger, IHubContext<MarketDataHub> hubContext) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger.LogDebug($"{nameof(MarketDataHub)} ctor hit");
        }

        public async Task Subscribed() {
            _logger.LogDebug($"{nameof(MarketDataHub)} - {nameof(Subscribed)} Hit");
            await _hubContext.Clients.All.SendAsync("Subscribed", $"Subscribed to data stream at {DateTime.Now}");
        }

        public async Task UnSubscribed() {
            _logger.LogDebug($"{nameof(MarketDataHub)} - {nameof(UnSubscribed)} Hit");
            await _hubContext.Clients.All.SendAsync("UnSubscribed", $"UnSubscribed to data stream at {DateTime.Now}");
        }

        public async Task QuoteReceived(Quote data) {
            _logger.LogDebug($"{nameof(MarketDataHub)} - {nameof(QuoteReceived)} Hit");
            await _hubContext.Clients.All.SendAsync("QuoteReceived", data);
        }

        public async Task TradeReceived(Trade data) {
            _logger.LogDebug($"{nameof(MarketDataHub)} - {nameof(TradeReceived)} Hit");
            await _hubContext.Clients.All.SendAsync("TradeReceived", data);
        }

        public async Task MinuteBarReceived(MinuteBar data) {
            _logger.LogDebug($"{nameof(MarketDataHub)} - {nameof(MinuteBarReceived)} Hit");
            await _hubContext.Clients.All.SendAsync("MinuteBarReceived", data);
        }
    }
}