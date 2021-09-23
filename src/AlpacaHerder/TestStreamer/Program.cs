using System;
using System.Threading;

namespace TestStreamer {
    class Program {
        static void Main(string[] args) {
            var streamer = new MarketDataStreamer();

            streamer.ListenAsync("SPY").GetAwaiter().GetResult();
            WaitHandle.WaitAll(streamer.WaitObjects, TimeSpan.FromSeconds(60));
            streamer.UnListenAsync("SPY").GetAwaiter().GetResult();
        }
    }
}