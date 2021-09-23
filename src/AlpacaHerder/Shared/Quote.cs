using System;

namespace AlpacaHerder.Shared {
    public record Quote {
        public string Symbol { get; init; }

        public ulong BidSize { get; init; }

        public decimal BidPrice { get; init; }

        public ulong AskSize { get; init; }

        public decimal AskPrice { get; init; }

        public DateTime TimestampUtc { get; init; }
    }
}
