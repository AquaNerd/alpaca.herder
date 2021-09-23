using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TestStreamer {
    public class MarketDataStreamer {
        public readonly AutoResetEvent[] WaitObjects;
        private readonly IAlpacaDataStreamingClient _client;
        private AuthStatus _authStatus;

        public MarketDataStreamer() {
            WaitObjects = new[] { new AutoResetEvent(false), new AutoResetEvent(false) };
            _client = Environments.Paper
                                  .GetAlpacaDataStreamingClient(new SecretKey("PKXJL66V7YRBSAE6114G", "0nmLZoFoWQg4wc0s8WzF1lsWiSpOCvwvLGJtWxi9"));
        }

        public async Task ListenAsync(string symbol, CancellationToken cancellationToken = default) {

            await ConnectAsync(cancellationToken);

            var subscriptions = GetSubscriptions(symbol);
            HookupEventsUponSubscribe(subscriptions);

            await _client.SubscribeAsync(subscriptions, cancellationToken);
        }

        public async Task UnListenAsync(string symbol, CancellationToken cancellationToken = default) {
            var tradeSubscription = _client.GetTradeSubscription(symbol);
            var quoteSubscription = _client.GetQuoteSubscription(symbol);
            var subscriptions = new List<IAlpacaDataSubscription> { tradeSubscription, quoteSubscription };

            await _client.UnsubscribeAsync(subscriptions, cancellationToken);

            await _client.DisconnectAsync(cancellationToken);
            _authStatus = AuthStatus.Unauthorized;
        }

        private List<IAlpacaDataSubscription> GetSubscriptions(string symbol) {
            var tradeSubscription = _client.GetTradeSubscription(symbol);
            var quoteSubscription = _client.GetQuoteSubscription(symbol);
            var subscriptions = new List<IAlpacaDataSubscription> { tradeSubscription, quoteSubscription };
            return subscriptions;
        }

        private void HookupEventsUponSubscribe(List<IAlpacaDataSubscription> subscriptions) {
            foreach (var subscription in subscriptions) {
                if (subscription.Streams.Any(s => s.ToLower().StartsWith("t."))) {
                    ((IAlpacaDataSubscription<ITrade>)subscription).Received += TradeSubscription_Received;
                }

                if (subscription.Streams.Any(s => s.ToLower().StartsWith("q."))) {
                    ((IAlpacaDataSubscription<IQuote>)subscription).Received += QuoteSubscription_Received;
                }
            }
        }

        private async Task ConnectAsync(CancellationToken cancellationToken = default) {
            _authStatus = await _client.ConnectAndAuthenticateAsync(cancellationToken);
            Console.WriteLine($"AuthStatus: {_authStatus}");
        }

        private void QuoteSubscription_Received(IQuote obj) {
            Console.WriteLine($"Quote received for the {obj.Symbol} contract \n\t {JsonSerializer.Serialize(obj)}");
            WaitObjects[1].Set();
        }

        private void TradeSubscription_Received(ITrade obj) {
            Console.WriteLine($"Trade received for the {obj.Symbol} contract \n\t {JsonSerializer.Serialize(obj)}");
            WaitObjects[0].Set();
        }
    }
}