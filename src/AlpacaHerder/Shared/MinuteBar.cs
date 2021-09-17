using System;

namespace AlpacaHerder.Shared {
    public class MinuteBar {

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public int Volume { get; set; }

        public string Symbol { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}