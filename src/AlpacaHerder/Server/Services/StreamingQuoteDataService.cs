using Alpaca.Markets;
using AlpacaHerder.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public class StreamingQuoteDataService : IStreamingDataService {

        private readonly ILogger<StreamingQuoteDataService> _logger;
        private readonly AlpacaConfig _alpacaConfig;
        private readonly IAlpacaDataStreamingClient _alpacaDataStreamingClient;
        private bool _isConnected;

        bool IStreamingDataService.IsConnected { get => _isConnected; set => _isConnected = value; }

        public StreamingQuoteDataService(ILogger<StreamingQuoteDataService> logger, IOptions<AlpacaConfig> alpacaConfig) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _alpacaConfig = alpacaConfig.Value ?? throw new ArgumentNullException(nameof(alpacaConfig));

            _alpacaDataStreamingClient = Environments.Paper.GetAlpacaDataStreamingClient(
                    new SecretKey(_alpacaConfig.ApiKey, _alpacaConfig.ApiSecret));

            _isConnected = false;
        }

        public async Task<IAlpacaDataSubscription> ConnectAndSubscribeAsync(string symbol, CancellationToken cancellationToken) {
            _alpacaDataStreamingClient.Connected += _alpacaDataStreamingClient_Connected;
            _alpacaDataStreamingClient.SocketClosed += _alpacaDataStreamingClient_SocketClosed;

            if (!_isConnected) {
                await _alpacaDataStreamingClient.ConnectAndAuthenticateAsync(cancellationToken);
            }

            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);
            subscription.Received += Subscription_Received;

            if (!subscription.Subscribed) {
                await _alpacaDataStreamingClient.SubscribeAsync(subscription, cancellationToken);
            }

            return subscription;
        }

        public async Task<IAlpacaDataSubscription> UnsubscribeAsync(string symbol, CancellationToken cancellationToken) {
            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);

            await _alpacaDataStreamingClient.UnsubscribeAsync(subscription, cancellationToken);

            return subscription;
        }

        public IAlpacaDataSubscription GetSubscription(string symbol) {
            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);

            return subscription;
        }

        private void _alpacaDataStreamingClient_SocketClosed() {
            _isConnected = false;
        }

        private void _alpacaDataStreamingClient_Connected(AuthStatus obj) {
            _isConnected = true;
        }

        private void Subscription_Received(IQuote obj) {
            _logger.LogDebug($"{obj.Symbol} - Bid({obj.BidSize}): {obj.BidPrice} Ask({obj.AskSize}): {obj.AskPrice} at {obj.TimestampUtc}");

            // TODO: fire back data to ui
        }
    }
}