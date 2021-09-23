using Alpaca.Markets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Services {
    public interface IStreamingDataService {

        Task ListenAsync(string symbol, CancellationToken cancellationToken = default);

        Task UnListenAsync(string symbol, CancellationToken cancellationToken = default);

        List<IAlpacaDataSubscription> GetSubscriptions(string symbol);

    }
}