using AlpacaHerder.GraphQL.Types;
using AlpacaHerder.Shared;
using GraphQL;
using GraphQL.Types;
using MediatR;

namespace AlpacaHerder.GraphQL.Queries;

public class MarketDataQuery : ObjectGraphType {

    public MarketDataQuery(IMediator mediator) {

        Field<MarketDataType>("marketData")
            .Arguments(new QueryArguments(
                new QueryArgument<StringGraphType> { Name = "symbol" }
            ))
            .ResolveAsync(async context => {
                var symbol = context.GetArgument<string>("symbol");
                var snapshot = await mediator.Send(new GetPrice(symbol));
                return new MarketData(snapshot.Symbol, snapshot.Trade.Price);
            });

    }

}
