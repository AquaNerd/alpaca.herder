using AlpacaHerder.Shared;
using GraphQL.Types;

namespace AlpacaHerder.GraphQL.Types;

public class MarketDataType : ObjectGraphType<MarketData> {
    public MarketDataType() {
        Field(x => x.Symbol, type: typeof(IdGraphType)).Description("Ticker symbol for the stock market instrument");
        Field(x => x.LastPrice, type: typeof(DecimalGraphType)).Description("Last price for the symbol");
    }
}
