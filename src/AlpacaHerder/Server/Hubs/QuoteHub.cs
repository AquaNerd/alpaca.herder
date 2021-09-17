using AlpacaHerder.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Hubs {
    public class QuoteHub : Hub {

        private IHubContext<QuoteHub> _hubContext;

        public QuoteHub(IHubContext<QuoteHub> hubContext) {
            _hubContext = hubContext;
        }

        public async Task SendMessage(Quote data) {  
            await _hubContext.Clients.All.SendAsync("ReceieveMessage", data);
        }
    }
}