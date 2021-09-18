using AlpacaHerder.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Hubs {
    public interface IQuoteHub {
        Task QuoteReceived(Quote data);
    }

    public class QuoteHub : Hub, IQuoteHub {

        private readonly IHubContext<QuoteHub> _hubContext;

        public QuoteHub(IHubContext<QuoteHub> hubContext) {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task QuoteReceived(Quote data) {  
            await _hubContext.Clients.All.SendAsync("QuoteReceived", data);
        }
    }
}