using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestStreamer {
    public interface IStreamingDataService : IAsyncDisposable {

        Task ListenAsync(string symbol, CancellationToken cancellationToken = default);

        Task UnListenAsync(string symbol, CancellationToken cancellationToken = default);

        List<IAlpacaDataSubscription> GetSubscriptions(string symbol);

    }
}