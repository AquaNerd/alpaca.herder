using Alpaca.Markets;
using AlpacaHerder.Server.Configuration;
using AlpacaHerder.Server.Hubs;
using AlpacaHerder.Shared;
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
        private AuthStatus _authStatus;
        private QuoteHub _broadcastHub;

        bool IStreamingDataService.IsConnected { get => _isConnected; }
        public AuthStatus AuthStatus { get => _authStatus; }

        public StreamingQuoteDataService(ILogger<StreamingQuoteDataService> logger, IOptions<AlpacaConfig> alpacaConfig, QuoteHub hub) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _alpacaConfig = alpacaConfig.Value;// ?? throw new ArgumentNullException(nameof(alpacaConfig));

            _alpacaDataStreamingClient = Environments.Paper.GetAlpacaDataStreamingClient(
                    new SecretKey(_alpacaConfig.ApiKey, _alpacaConfig.ApiSecret));

            _isConnected = false;

            _alpacaDataStreamingClient.Connected += _alpacaDataStreamingClient_Connected;
            _alpacaDataStreamingClient.SocketOpened += _alpacaDataStreamingClient_SocketOpened;
            _alpacaDataStreamingClient.SocketClosed += _alpacaDataStreamingClient_SocketClosed;

            _broadcastHub = hub;
        }

        public async Task<IAlpacaDataSubscription> SubscribeAsync(string symbol, CancellationToken cancellationToken = default) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            _logger.LogDebug($"Current Auth Status: {_authStatus}");

            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);
            subscription.Received += Subscription_Received;

            if (!subscription.Subscribed) {
                await _alpacaDataStreamingClient.SubscribeAsync(subscription, cancellationToken = default);
            }

            subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);

            await _broadcastHub.SendMessage(new Quote {
                TimestampUTC = DateTime.UtcNow,
                Symbol = "slqt",
                BidPrice = 10.01M,
                BidSize = 5,
                AskPrice = 11.03M,
                AskSize = 20
            });

            return subscription;
        }

        public async Task<IAlpacaDataSubscription> UnsubscribeAsync(string symbol, CancellationToken cancellationToken = default) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);
            await _alpacaDataStreamingClient.UnsubscribeAsync(subscription, cancellationToken);

            subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);

            return subscription;
        }

        public async Task<IAlpacaDataSubscription> GetSubscriptionAsync(string symbol, CancellationToken cancellationToken = default) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            var subscription = _alpacaDataStreamingClient.GetQuoteSubscription(symbol);

            return subscription;
        }

        private async Task<AuthStatus> ConnectAndAuthorizeAsync(CancellationToken cancellationToken) {
            if (_authStatus == AuthStatus.Unauthorized) {
                _logger.LogDebug($"Starting - Connect and Auth - Auth Status: {_authStatus}");
                _authStatus = await _alpacaDataStreamingClient.ConnectAndAuthenticateAsync(cancellationToken);
                _logger.LogDebug($"Completed - Connect and Auth - Auth Status: {_authStatus}");
            }

            return _authStatus;
        }

        private void _alpacaDataStreamingClient_SocketClosed() {
            _isConnected = false;
        }

        private void _alpacaDataStreamingClient_SocketOpened() {
            _isConnected = true;
        }

        private void _alpacaDataStreamingClient_Connected(AuthStatus obj) {
            _authStatus = obj;
            _logger.LogDebug($"Connected - Auth Status: {_authStatus}");
            _isConnected = true;
        }

        private async void Subscription_Received(IQuote obj) {
            _logger.LogInformation($"{obj.Symbol}({obj.TimestampUtc}) - Bid: {obj.BidPrice}({obj.BidSize}) Ask: {obj.AskPrice}({obj.AskSize})");

            await _broadcastHub.SendMessage(new Quote {
                TimestampUTC = obj.TimestampUtc,
                Symbol = obj.Symbol,
                BidPrice = obj.BidPrice,
                BidSize = obj.BidSize,
                AskPrice = obj.AskPrice,
                AskSize = obj.AskSize
            });
        }
    }
}