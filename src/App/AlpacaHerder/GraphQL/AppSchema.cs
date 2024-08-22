using AlpacaHerder.GraphQL.Queries;
using GraphQL.Types;

namespace AlpacaHerder.GraphQL;
public class AppSchema : Schema {

    public AppSchema(MarketDataQuery marketDataQuery) {
        Query = marketDataQuery;
    }

}
