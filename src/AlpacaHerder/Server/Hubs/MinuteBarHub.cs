using AlpacaHerder.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace AlpacaHerder.Server.Hubs {
    public class MinuteBarHub : Hub {
        public async Task SendMessage(MinuteBar data, CancellationToken cancellationToken = default) {
            await Clients.All.SendAsync("ReceieveMessage", data, cancellationToken);
        }
    }
}