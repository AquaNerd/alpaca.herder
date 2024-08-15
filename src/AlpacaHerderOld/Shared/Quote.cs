using System;

namespace AlpacaHerder.Shared {
    public record Quote {
        public string Symbol { get; init; }

        public decimal BidSize { get; init; }

        public decimal BidPrice { get; init; }

        public decimal AskSize { get; init; }

        public decimal AskPrice { get; init; }

        public DateTime TimestampUtc { get; init; }
    }
}
