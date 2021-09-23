﻿using Alpaca.Markets;
using AlpacaHerder.Server.Hubs;
using AlpacaHerder.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public class StreamingDataService : IStreamingDataService, IAsyncDisposable {
        private readonly ILogger<StreamingDataService> _logger;
        private readonly IAlpacaDataStreamingClient _client;
        private AuthStatus _authStatus;
        private readonly IMarketDataHub _hub;
        private IAlpacaDataSubscription<ITrade> _tradeSubscription;
        private IAlpacaDataSubscription<IQuote> _quoteSubscription;

        public StreamingDataService(ILogger<StreamingDataService> logger, IAlpacaDataStreamingClient client, IMarketDataHub hub) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _authStatus = AuthStatus.Unauthorized;
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _logger.LogDebug($"{nameof(StreamingDataService)} ctor hit");
        }

        public async Task ListenAsync(string symbol, CancellationToken cancellationToken = default) {

            await ConnectAsync(cancellationToken);

            var subscriptions = GetSubscriptions(symbol);

            if (!subscriptions.All(s => s.Subscribed)) {

                HookupEventsUponSubscribe();

                await _client.SubscribeAsync(subscriptions, cancellationToken);

                await _hub.Subscribed();
            }
        }

        public async Task UnListenAsync(string symbol, CancellationToken cancellationToken = default) {
            _tradeSubscription = _client.GetTradeSubscription(symbol);
            _quoteSubscription = _client.GetQuoteSubscription(symbol);
            var subscriptions = new List<IAlpacaDataSubscription> { _tradeSubscription, _quoteSubscription };

            if (subscriptions.All(s => s.Subscribed)) {

                await _client.UnsubscribeAsync(subscriptions, cancellationToken);

                await _hub.UnSubscribed();
            }
        }

        private void HookupEventsUponSubscribe() {
            _tradeSubscription.Received += TradeSubscription_Received;
            _logger.LogDebug($"{string.Join(", ", _tradeSubscription.Streams)} - Hooked up {nameof(TradeSubscription_Received)} to Subscription Received Event");

            _quoteSubscription.Received += QuoteSubscription_Received;
            _logger.LogDebug($"{string.Join(", ", _quoteSubscription.Streams)} - Hooked up {nameof(QuoteSubscription_Received)} to Subscription Received Event");
        }

        public List<IAlpacaDataSubscription> GetSubscriptions(string symbol) {

            var tradeSubscription = _client.GetTradeSubscription(symbol);
            _tradeSubscription = tradeSubscription;
            _logger.LogTrace($"{nameof(tradeSubscription)} - Streams: {string.Join(", ", tradeSubscription.Streams)} Subscribed: {tradeSubscription.Subscribed}");

            var quoteSubscription = _client.GetQuoteSubscription(symbol);
            _quoteSubscription = quoteSubscription;
            _logger.LogTrace($"{nameof(quoteSubscription)} - Streams: {string.Join(", ", quoteSubscription.Streams)} Subscribed: {quoteSubscription.Subscribed}");

            var subscriptions = new List<IAlpacaDataSubscription> { _tradeSubscription, _quoteSubscription };

            return subscriptions;
        }

        private async Task ConnectAsync(CancellationToken cancellationToken = default) {
            if (_authStatus == AuthStatus.Unauthorized) {
                _authStatus = await _client.ConnectAndAuthenticateAsync(cancellationToken);
                _logger.LogTrace($"AuthStatus: {_authStatus}");
            }
        }

        private async void QuoteSubscription_Received(IQuote obj) {
            var quote = new Quote {
                TimestampUtc = obj.TimestampUtc
                ,
                Symbol = obj.Symbol
                ,
                BidPrice = obj.BidPrice
                ,
                BidSize = obj.BidSize
                ,
                AskPrice = obj.AskPrice
                ,
                AskSize = obj.AskSize
            };

            _logger.LogInformation($"{quote}");

            await _hub.QuoteReceived(quote);
        }

        private async void TradeSubscription_Received(ITrade obj) {
            var trade = new Trade(obj.Symbol, obj.Price, obj.Size, obj.TradeId, obj.TimestampUtc);

            _logger.LogInformation($"{trade}");

            await _hub.TradeReceived(trade);
        }
        
        public async ValueTask DisposeAsync() {
            await _client.DisconnectAsync(default);
            _authStatus = AuthStatus.Unauthorized;
            _logger.LogTrace($"AuthStatus: {_authStatus}");
            _client?.Dispose();
        }
    }
}