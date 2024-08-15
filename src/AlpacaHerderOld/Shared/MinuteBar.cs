using System;

namespace AlpacaHerder.Shared {
    public record MinuteBar {

        public decimal Open { get; init; }

        public decimal High { get; init; }

        public decimal Low { get; init; }

        public decimal Close { get; init; }

        public int Volume { get; init; }

        public string Symbol { get; init; }

        public DateTime TimeUtc { get; init; }
    }
}