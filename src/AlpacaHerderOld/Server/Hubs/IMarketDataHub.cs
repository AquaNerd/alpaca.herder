using AlpacaHerder.Shared;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Hubs {
    public interface IMarketDataHub {

        Task Subscribed();

        Task UnSubscribed();

        Task QuoteReceived(Quote data);

        Task TradeReceived(Trade data);

        Task MinuteBarReceived(MinuteBar data);
    }
}