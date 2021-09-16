using Alpaca.Markets;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public interface IStreamingDataService {

        bool IsConnected { get; }

        AuthStatus AuthStatus { get; }

        Task<IAlpacaDataSubscription> SubscribeAsync(string symbol, CancellationToken cancellationToken);

        Task<IAlpacaDataSubscription> UnsubscribeAsync(string symbol, CancellationToken cancellationToken);

        Task<IAlpacaDataSubscription> GetSubscriptionAsync(string symbol, CancellationToken cancellationToken);

    }
}
