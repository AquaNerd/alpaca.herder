using Alpaca.Markets;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public interface IStreamingDataService {

        bool IsConnected { get; set; }

        Task<IAlpacaDataSubscription> ConnectAndSubscribeAsync(string symbol, CancellationToken cancellationToken);

        Task<IAlpacaDataSubscription> UnsubscribeAsync(string symbol, CancellationToken cancellationToken);

        IAlpacaDataSubscription GetSubscription(string symbol);

    }
}
