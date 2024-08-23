using Alpaca.Markets;
using AlpacaHerder.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace AlpacaHerder;

public class GetPrice : IRequest<ISnapshot> {

    public readonly string Symbol;

    public GetPrice(string symbol) {
        Symbol = symbol;
    }

    public class ControllerHandler : IRequestHandler<GetPrice, ISnapshot> {

        private readonly IAlpacaDataClient _alpacaDataClient;
        private readonly AlpacaConfig _alpacaConfig;

        public ControllerHandler(IOptions<AlpacaConfig> alpacaConfigOptions) {
            _alpacaConfig = alpacaConfigOptions.Value ?? throw new ArgumentNullException(nameof(alpacaConfigOptions));

            // TODO: Dependency Inject this client instead of creating a new instance of
            _alpacaDataClient = Alpaca.Markets.Environments.Paper.GetAlpacaDataClient(
                new SecretKey(_alpacaConfig.ApiKey, _alpacaConfig.ApiSecret));
        }


        public async Task<ISnapshot> Handle(GetPrice request, CancellationToken cancellationToken)
            => await _alpacaDataClient.GetSnapshotAsync(new LatestMarketDataRequest(request.Symbol), cancellationToken);
    }
}
