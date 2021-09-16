using Alpaca.Markets;
using AlpacaHerder.Server.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public class StreamingDataService : IStreamingDataService {

        private readonly ILogger<StreamingDataService> _logger;
        private readonly AlpacaConfig _alpacaConfig;
        private readonly IAlpacaDataStreamingClient _alpacaDataStreamingClient;
        private bool _isConnected;
        private AuthStatus _authStatus;
        private IAlpacaDataSubscription<IBar> _subscription;

        bool IStreamingDataService.IsConnected { get => _isConnected; }
        public AuthStatus AuthStatus { get => _authStatus; }

        public StreamingDataService(ILogger<StreamingDataService> logger, IOptions<AlpacaConfig> alpacaConfig) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _alpacaConfig = alpacaConfig.Value ?? throw new ArgumentNullException(nameof(alpacaConfig));

            _alpacaDataStreamingClient = Environments.Paper.GetAlpacaDataStreamingClient(
                    new SecretKey(_alpacaConfig.ApiKey, _alpacaConfig.ApiSecret));

            _isConnected = false;

            _alpacaDataStreamingClient.Connected += _alpacaDataStreamingClient_Connected;
            _alpacaDataStreamingClient.SocketOpened += _alpacaDataStreamingClient_SocketOpened;
            _alpacaDataStreamingClient.SocketClosed += _alpacaDataStreamingClient_SocketClosed;
        }

        public async Task<IAlpacaDataSubscription> SubscribeAsync(string symbol, CancellationToken cancellationToken) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            _logger.LogDebug($"Current Auth Status: {_authStatus}");

            _subscription = _alpacaDataStreamingClient.GetMinuteBarSubscription(symbol);
            _subscription.Received += Subscription_Received;

            if (!_subscription.Subscribed) {
                await _alpacaDataStreamingClient.SubscribeAsync(_subscription, cancellationToken);
            }

            _subscription = _alpacaDataStreamingClient.GetMinuteBarSubscription(symbol);

            return _subscription;
        }

        public async Task<IAlpacaDataSubscription> UnsubscribeAsync(string symbol, CancellationToken cancellationToken) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            await _alpacaDataStreamingClient.UnsubscribeAsync(_subscription, cancellationToken);

            _subscription = _alpacaDataStreamingClient.GetMinuteBarSubscription(symbol);

            return _subscription;
        }

        public async Task<IAlpacaDataSubscription> GetSubscriptionAsync(string symbol, CancellationToken cancellationToken) {
            await ConnectAndAuthorizeAsync(cancellationToken);

            _subscription = _alpacaDataStreamingClient.GetMinuteBarSubscription(symbol);

            return _subscription;
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

        private void Subscription_Received(IBar obj) {
            _logger.LogInformation($"{obj.Symbol}({obj.TimeUtc}) - O: {obj.Open} H: {obj.High} L: {obj.Low} C: {obj.Close}");

            // TODO: fire back data to ui
        }
    }
}