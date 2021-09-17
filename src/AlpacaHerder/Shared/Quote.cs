using System;

namespace AlpacaHerder.Shared {
    public class Quote {
        public string Symbol { get; set; }

        public ulong BidSize { get; set; }

        public decimal BidPrice { get; set; }

        public ulong AskSize { get; set; }

        public decimal AskPrice { get; set; }

        public DateTime TimestampUTC { get; set; }
    }
}
